using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Messages;
using NockChat.Models.Pagination;

namespace NockChat.Services.HTTP.Requests.Messages
{
    /// <summary>
    /// Интерфейс для получения сообщений комнаты с сервера
    /// </summary>
    public interface IMessageRequestsService
    {
        /// <summary>
        /// Возвращает страницу сообщений текущей комнаты
        /// </summary>
        /// <param name="token">Токен сессии комнаты</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Количество сообщений на странице</param>
        Task<PagedResult<MessageModel>> GetMessagesAsync(string token, int page = 1, int pageSize = 50, CancellationToken ct = default);
    }
}