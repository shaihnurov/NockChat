using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Messages;

namespace NockChat.Services.HTTP.Requests.Messages
{
    public interface IMessageRequestsService
    {
        Task<List<MessageModel>?> GetMessagesAsync(string token, int page = 1, int pageSize = 50, CancellationToken ct = default);
    }
}