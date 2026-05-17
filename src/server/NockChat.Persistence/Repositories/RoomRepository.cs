using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;

namespace NockChat.Persistence.Repositories
{
    public class RoomRepository(AppDbContext db) : IRoomRepository
    {
        public async Task<Room> CreateAsync(Room room, CancellationToken ct = default)
        {
            db.Rooms.Add(room);
            await db.SaveChangesAsync(ct);
            return room;
        }
    }
}