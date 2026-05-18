using Microsoft.EntityFrameworkCore;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    public class RoomRepository(AppDbContext db) : IRoomRepository
    {
        public async Task<Room> CreateAsync(Room room, CancellationToken ct = default)
        {
            await db.Rooms.AddAsync(room, ct);
            await db.SaveChangesAsync(ct);
            return room;
        }

        public async Task<Room?> GetByAccessCodeAsync(string accessCode, CancellationToken ct = default)
            => await db.Rooms.FirstOrDefaultAsync(r => r.AccessCode == accessCode, ct);
    }
}