using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Rooms;
using NockChat.Models.Sessions;

namespace NockChat.Services.HTTP.Requests.Rooms
{
    public interface IRoomRequestsService
    {
        Task<RoomSession?> CreateRoom(string roomName, string userName, CancellationToken ct = default);
        Task<RoomSession?> JoinRoom(string accessCode, string userName, CancellationToken ct = default);
        Task DeleteRoom(string token, CancellationToken ct = default);
        Task<IReadOnlyList<RoomUserModel>?> GetRoomUsers(string token, CancellationToken ct = default);
        Task<InviteCodeModel?> GetInviteCodeRoom(string token, CancellationToken ct = default);
    }
}