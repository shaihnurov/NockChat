using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NockChat.Services.HTTP
{
    /// <summary>
    /// Интерфейс для выполнения HTTP-запросов
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Выполняет HTTP GET-запрос
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint, CancellationToken ct = default);

        /// <summary>
        /// Выполняет HTTP GET-запрос
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint, string token, CancellationToken ct = default);

        /// <summary>
        /// Выполняет HTTP POST-запрос с JSON-телом
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> PostAsync<T>(string endpoint, object body, CancellationToken ct = default);

        /// <summary>
        /// Выполняет HTTP POST-запрос с JSON-телом
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> PostAsync<T>(string endpoint, object body, string token, CancellationToken ct = default);

        /// <summary>
        /// Выполняет HTTP POST-запрос с multipart/form-data содержимым.
        /// Dispose переданного <paramref name="multipart"/> — ответственность вызывающего кода.
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent multipart, CancellationToken ct = default);

        /// <summary>
        /// Выполняет HTTP PUT-запрос с JSON-телом
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> PutAsync<T>(string endpoint, object body, CancellationToken ct = default);

        /// <summary>
        /// Выполняет HTTP PATCH-запрос с JSON-телом
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> PatchAsync<T>(string endpoint, object body, CancellationToken ct = default);

        /// <summary>
        /// Выполняет HTTP DELETE-запрос, опционально с JSON-телом
        /// </summary>
        Task<(bool Success, T? Data, string? ErrorMessage)> DeleteAsync<T>(string endpoint, object? body = null, CancellationToken ct = default);

        /// <summary>
        /// Выполняет GET-запрос и возвращает поток данных без загрузки всего содержимого в память.
        /// Вызывающий код обязан вызвать Dispose на возвращённом потоке.
        /// </summary>
        Task<Stream> GetStreamAsync(string requestUri, CancellationToken ct = default);
    }
}