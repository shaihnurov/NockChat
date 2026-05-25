using Microsoft.EntityFrameworkCore;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    /// <summary>
    /// Реализация <see cref="IParticipantKeyRepository"/>
    /// </summary>
    public class ParticipantKeyRepository(AppDbContext db) : IParticipantKeyRepository
    {
        /// <inheritdoc/>
        public async Task UpsertAsync(ParticipantKey key, CancellationToken ct = default)
        {
            var existing = await db.ParticipantKeys.FirstOrDefaultAsync(k => k.RoomId == key.RoomId && k.ChatUserId == key.ChatUserId, ct);

            if (existing is null)
                await db.ParticipantKeys.AddAsync(key, ct);
            else
            {
                existing.EphemeralPublicKey = key.EphemeralPublicKey;
                existing.PublishedAt = key.PublishedAt;
            }

            await db.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ParticipantKey>> GetRoomKeysAsync(int roomId, int excludeChatUserId, CancellationToken ct = default)
            => await db.ParticipantKeys.AsNoTracking().Where(k => k.RoomId == roomId && k.ChatUserId != excludeChatUserId).Include(k => k.ChatUser).ToListAsync(ct);

        /// <inheritdoc/>
        public async Task DeleteAsync(int chatUserId, int roomId, CancellationToken ct = default)
        {
            await db.ParticipantKeys.Where(k => k.ChatUserId == chatUserId && k.RoomId == roomId).ExecuteDeleteAsync(ct);
        }
    }
}