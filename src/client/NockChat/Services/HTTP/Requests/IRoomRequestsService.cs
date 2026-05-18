using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Rooms;

namespace NockChat.Services.HTTP.Requests
{
    public interface IRoomRequestsService
    {
        Task<RoomModel?> CreateRoom(string roomName, CancellationToken ct);
    }
}