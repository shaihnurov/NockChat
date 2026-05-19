using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NockChat.Models.Messages;
using NockChat.Models.Pagination;

namespace NockChat.Services.HTTP.Requests.Messages
{
    public class MessageRequestsService(ILogger<MessageRequestsService> logger, IHttpService httpService) : IMessageRequestsService
    {
        public async Task<PagedResult<MessageModel>?> GetMessagesAsync(string token, int page = 1, int pageSize = 50, CancellationToken ct = default)
        {
            var (success, response, error) = await httpService.GetAsync<PagedResult<MessageModel>>($"/api/v1/messages?page={page}&pageSize={pageSize}", token, ct);

            if (!success)
            {
                logger.LogWarning("Ошибка загрузки сообщений: {Error}", error);
                return null;
            }

            return response;
        }
    }
}