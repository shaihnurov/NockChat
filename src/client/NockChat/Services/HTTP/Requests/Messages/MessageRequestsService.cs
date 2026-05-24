using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Messages;
using NockChat.Models.Pagination;

namespace NockChat.Services.HTTP.Requests.Messages
{
    /// <summary>
    /// Реализация HTTP-запросов для получения сообщений комнаты
    /// </summary>
    public class MessageRequestsService(IHttpService httpService) : IMessageRequestsService
    {
        /// <inheritdoc/>
        public async Task<PagedResult<MessageModel>> GetMessagesAsync(string token, int page = 1, int pageSize = 50, CancellationToken ct = default)
            => await httpService.GetAsync<PagedResult<MessageModel>>($"/api/v1/messages?page={page}&pageSize={pageSize}", token, ct);
    }
}