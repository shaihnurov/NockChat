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
        public Task<(bool Success, T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Get, endpoint, ct: ct);

        /// <inheritdoc />
        public Task<(bool Success, T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint, string token, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Get, endpoint, token: token, ct: ct);

        /// <inheritdoc />
        public Task<(bool Success, T? Data, string? ErrorMessage)> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Post, endpoint, body, ct: ct);

        /// <inheritdoc />
        public Task<(bool Success, T? Data, string? ErrorMessage)> PostAsync<T>(string endpoint, object body, string token, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Post, endpoint, body: body, token: token, ct: ct);

        /// <inheritdoc />
        public Task<(bool Success, T? Data, string? ErrorMessage)> PutAsync<T>(string endpoint, object body, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Put, endpoint, body, ct: ct);

        /// <inheritdoc />
        public Task<(bool Success, T? Data, string? ErrorMessage)> PatchAsync<T>(string endpoint, object body, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Patch, endpoint, body, ct: ct);

        /// <inheritdoc />
        public Task<(bool Success, T? Data, string? ErrorMessage)> DeleteAsync<T>(string endpoint, object? body = null, CancellationToken ct = default)
            => SendAsync<T>(HttpMethod.Delete, endpoint, body, ct: ct);

        /// <inheritdoc />
        public async Task<(bool Success, T? Data, string? ErrorMessage)> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent multipart, CancellationToken ct = default)
            => await SendAsync<T>(HttpMethod.Post, endpoint, httpContent: multipart, ct: ct);

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
        private async Task<(bool Success, T? Data, string? ErrorMessage)> SendAsync<T>(HttpMethod method, string endpoint, object? body = null, 
            HttpContent? httpContent = null, string? token = null, CancellationToken ct = default)
        {
            if (!networkService.IsOnline)
            {
                logger.LogDebug("Запрос [{Method} {Endpoint}] пропущен — нет сети", method, endpoint);
                return (false, default, NetworkErrors.NoConnection);
            }

            try
            {
                using var request = BuildRequest(method, endpoint, body, httpContent, token);

                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

                return await ProcessResponseAsync<T>(response, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return (false, default, "Запрос отменён");
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Таймаут запроса к серверу");
                return (false, default, "Таймаут запроса к серверу");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Ошибка подключения к серверу");
                return (false, default, "Ошибка подключения к серверу");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Неизвестная ошибка при выполнении запроса");
                return (false, default, "Неизвестная ошибка при выполнении запроса");
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

        private async Task<(bool Success, T? Data, string? ErrorMessage)> ProcessResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
        {
            var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

#if DEBUG
            logger.LogInformation("{Body}", System.Text.RegularExpressions.Regex.Unescape(body));
#endif

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = ExtractErrorMessage(body);
                logger.LogError("[{StatusCode}]: {ErrorMessage}", response.StatusCode, errorMessage);
                return (false, default, errorMessage);
            }

            try
            {
                return (true, JsonConvert.DeserializeObject<T>(body, SerializerSettings), null);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Ошибка десериализации JSON: {Body}", body);
                return (false, default, "Ошибка десериализации ответа от сервера");
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
                return responseBody.Length > 200
                    ? string.Concat(responseBody.AsSpan(0, 200), "...")
                    : responseBody;
            }
        }
    }
}