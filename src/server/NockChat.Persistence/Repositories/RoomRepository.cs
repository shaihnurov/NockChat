using Microsoft.EntityFrameworkCore;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    /// <summary>
    /// Реализация <see cref="IRoomRepository"/>
    /// </summary>
    public class RoomRepository(AppDbContext db) : IRoomRepository
    {
        /// <inheritdoc/>
        public async Task<Room> CreateAsync(Room room, CancellationToken ct = default)
        {
            await db.Rooms.AddAsync(room, ct);
            await db.SaveChangesAsync(ct);
            return room;
        }

        /// <inheritdoc/>
        public async Task<Room?> GetByAccessCodeAsync(string accessCode, CancellationToken ct = default)
            => await db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.AccessCode == accessCode, ct);

        /// <inheritdoc/>
        public async Task DeleteAsync(int roomId, CancellationToken ct = default)
        {
            var deleted = await db.Rooms.Where(r => r.Id == roomId).ExecuteDeleteAsync(ct);

            if (deleted == 0)
                throw new NotFoundException($"Комната {roomId} не найдена");
        }
    }
}