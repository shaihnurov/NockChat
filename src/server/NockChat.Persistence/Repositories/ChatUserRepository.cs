using Microsoft.EntityFrameworkCore;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    public class ChatUserRepository(AppDbContext db) : IChatUserRepository
    {
        public async Task<bool> ExistsAsync(int roomId, string username, CancellationToken ct = default)
            => await db.ChatUsers.AsNoTracking().AnyAsync(u => u.RoomId == roomId && u.Username == username, ct);

        public async Task<ChatUser> CreateAsync(ChatUser chatUser, CancellationToken ct = default)
        {
            await db.ChatUsers.AddAsync(chatUser, ct);
            await db.SaveChangesAsync(ct);
            return chatUser;
        }

        public async Task<ChatUser?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.ChatUsers.FindAsync([id], ct);
    }
}