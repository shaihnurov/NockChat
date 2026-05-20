using Microsoft.EntityFrameworkCore;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    /// <summary>
    /// Реализация <see cref="IMessageRepository"/>
    /// </summary>
    public class MessageRepository(AppDbContext db) : IMessageRepository
    {
        /// <inheritdoc/>
        public async Task<Message> CreateAsync(Message message, CancellationToken ct = default)
        {
            await db.Messages.AddAsync(message, ct);
            await db.SaveChangesAsync(ct);
            return message;
        }

        /// <inheritdoc/>
        public async Task<(List<Message> Messages, int TotalCount)> GetByRoomAsync(int roomId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = db.Messages.Where(m => m.RoomId == roomId);
            var totalCount = await query.CountAsync(ct);

            var messages = await query.AsNoTracking().OrderBy(m => m.SentAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(m => new Message
                {
                    Id = m.Id,
                    Text = m.Text,
                    SentAt = m.SentAt,
                    ChatUserId = m.ChatUserId,
                    ChatUser = new ChatUser { Username = m.ChatUser.Username }
                })
                .ToListAsync(ct);

            return (messages, totalCount);
        }
    }
}