using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NockChat.Services.Common.Exceptions;
using NockChat.Services.HTTP.Network;
using NockChat.Services.HTTP.Options;

namespace NockChat.Services.HTTP
{
    /// <summary>
    /// Кроссплатформенный сервис для выполнения HTTP-запросов
    /// </summary>
    public sealed class HttpService(HttpClient httpClient, ILogger<HttpService> logger, IOptions<HttpServiceOptions> options, INetworkService networkService) : IHttpService
    {
        private readonly HttpServiceOptions _options = options.Value;

        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters = [new StringEnumConverter()]
        };

        /// <inheritdoc />
        public Task<T> GetAsync<T>(string endpoint, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Get, endpoint, ct: ct);

        /// <inheritdoc />
        public Task<T> GetAsync<T>(string endpoint, string token, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Get, endpoint, token: token, ct: ct);

        /// <inheritdoc />
        public Task<T> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Post, endpoint, body, ct: ct);

        /// <inheritdoc />
        public Task<T> PostAsync<T>(string endpoint, object body, string token, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Post, endpoint, body: body, token: token, ct: ct);

        /// <inheritdoc />
        public Task<T> PutAsync<T>(string endpoint, object body, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Put, endpoint, body, ct: ct);

        /// <inheritdoc />
        public Task<T> PatchAsync<T>(string endpoint, object body, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Patch, endpoint, body, ct: ct);

        /// <inheritdoc />
        public Task<T> DeleteAsync<T>(string endpoint, object? body = null, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Delete, endpoint, body, ct: ct);

        /// <inheritdoc />
        public Task<T> DeleteAsync<T>(string endpoint, string token, object? body = null, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Delete, endpoint, token: token, body: body, ct: ct);

        /// <inheritdoc />
        public async Task<Stream> GetStreamAsync(string requestUri, CancellationToken ct = default)
        {
            if (!networkService.IsOnline)
                throw new InvalidOperationException(NetworkErrors.NoConnection);

            var url = Uri.IsWellFormedUriString(requestUri, UriKind.Absolute) ? requestUri : $"{_options.BaseUrl}{requestUri}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Единая точка отправки всех HTTP-запросов
        /// </summary>
        private async Task<T> SendAsync<T>(HttpMethod method, string endpoint, object? body = null,
            string? token = null, CancellationToken ct = default)
        {
            if (!networkService.IsOnline)
                throw new NetworkException(NetworkErrors.NoConnection);

            try
            {
                using var request = BuildRequest(method, endpoint, body, null, token);
                using var response = await httpClient.SendAsync(request, ct);

                return await ProcessResponseAsync<T>(response, ct);
            }
            catch (NetworkException)
            {
                throw;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw new NetworkException(NetworkErrors.Cancelled);
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Таймаут запроса [{Method} {Endpoint}]", method, endpoint);
                throw new NetworkException(NetworkErrors.Timeout);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Ошибка подключения [{Method} {Endpoint}]", method, endpoint);
                throw new NetworkException(NetworkErrors.ServerUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Неизвестная ошибка [{Method} {Endpoint}]", method, endpoint);
                throw new NetworkException(NetworkErrors.UnknownError);
            }
        }

        private HttpRequestMessage BuildRequest(HttpMethod method, string endpoint, object? body, HttpContent? httpContent, string? token)
        {
            var request = new HttpRequestMessage(method, BuildUrl(endpoint));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (httpContent is not null)
                request.Content = httpContent;
            else if (body is not null)
                request.Content = SerializeToJson(body);

            return request;
        }

        private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
        {
            var body = await response.Content.ReadAsStringAsync(ct);

#if DEBUG
            logger.LogInformation("{Body}", System.Text.RegularExpressions.Regex.Unescape(body));
#endif

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = ExtractErrorMessage(body);
                throw new ServerException(errorMessage, (int)response.StatusCode);
            }

            if (typeof(T) == typeof(object))
                return default!;

            try
            {
                return JsonConvert.DeserializeObject<T>(body, SerializerSettings) ?? throw new DeserializationException(NetworkErrors.DeserializationError);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Ошибка десериализации: {Body}", body);
                throw new DeserializationException(NetworkErrors.DeserializationError);
            }
        }

        private string BuildUrl(string endpoint) =>
            Uri.IsWellFormedUriString(endpoint, UriKind.Absolute) ? endpoint : $"{_options.BaseUrl}{endpoint}";

        private static StringContent SerializeToJson(object data)
        {
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = [new StringEnumConverter()]
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static string ExtractErrorMessage(string? responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return "Неизвестная ошибка";

            try
            {
                var json = JObject.Parse(responseBody);
                return json["message"]?.ToString()
                    ?? json["error"]?.ToString()
                    ?? json["detail"]?.ToString()
                    ?? "Ошибка запроса";
            }
            catch
            {
                return responseBody.Length > 200 ? string.Concat(responseBody.AsSpan(0, 200), "...") : responseBody;
            }
        }
    }
}