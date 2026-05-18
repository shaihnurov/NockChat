using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Sessions;

namespace NockChat.Services.HTTP.Requests.Rooms
{
    public interface IRoomRequestsService
    {
        Task<RoomSession?> CreateRoom(string roomName, string userName, CancellationToken ct);
        Task<RoomSession?> JoinRoom(string accessCode, string userName, CancellationToken ct);
    }
}