using Microsoft.EntityFrameworkCore;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    public class MessageRepository(AppDbContext db) : IMessageRepository
    {
        public async Task<Message> CreateAsync(Message message, CancellationToken ct = default)
        {
            await db.Messages.AddAsync(message, ct);
            await db.SaveChangesAsync(ct);
            return message;
        }

        public async Task<List<Message>> GetByRoomAsync(int roomId, int page, int pageSize, CancellationToken ct = default)
            => await db.Messages.Where(m => m.RoomId == roomId).Include(m => m.ChatUser).OrderByDescending(m => m.SentAt).Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync(ct);
    }
}