using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> CreateAsync(Room room, CancellationToken ct = default);
    }
}