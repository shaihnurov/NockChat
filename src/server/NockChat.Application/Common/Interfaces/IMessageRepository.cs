using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateAsync(Message message, CancellationToken ct = default);
        Task<List<Message>> GetByRoomAsync(int roomId, int page, int pageSize, CancellationToken ct = default);
    }
}