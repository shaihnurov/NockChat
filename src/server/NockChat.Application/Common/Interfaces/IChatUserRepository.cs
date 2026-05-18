using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Interfaces
{
    public interface IChatUserRepository
    {
        Task<bool> ExistsAsync(int roomId, string username, CancellationToken ct = default);
        Task<ChatUser> CreateAsync(ChatUser chatUser, CancellationToken ct = default);
        Task<ChatUser?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}