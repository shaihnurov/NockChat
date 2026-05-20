using Microsoft.EntityFrameworkCore;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    /// <summary>
    /// Реализация <see cref="IChatUserRepository"/>
    /// </summary>
    public class ChatUserRepository(AppDbContext db) : IChatUserRepository
    {
        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(int roomId, string username, CancellationToken ct = default)
            => await db.ChatUsers.AsNoTracking().AnyAsync(u => u.RoomId == roomId && u.Username == username, ct);

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ChatUser>> GetRoomUsersAsync(int roomId, CancellationToken ct = default)
            => await db.ChatUsers.AsNoTracking().Where(u => u.RoomId == roomId)
                .Select(u => new ChatUser
                {
                    Username = u.Username,
                    JoinedAt = u.JoinedAt
                }).ToListAsync(ct);

        /// <inheritdoc/>
        public async Task<ChatUser> CreateAsync(ChatUser chatUser, CancellationToken ct = default)
        {
            await db.ChatUsers.AddAsync(chatUser, ct);
            await db.SaveChangesAsync(ct);
            return chatUser;
        }

        /// <inheritdoc/>
        public async Task<ChatUser?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.ChatUsers.FindAsync([id], ct);
    }
}