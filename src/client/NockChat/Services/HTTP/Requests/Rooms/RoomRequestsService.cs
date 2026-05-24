using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Rooms;
using NockChat.Models.Sessions;
using NockChat.Services.Common.DataStorage.Sessions;

namespace NockChat.Services.HTTP.Requests.Rooms
{
    /// <summary>
    /// Реализация HTTP-запросов для управления комнатами
    /// </summary>
    public class RoomRequestsService(IHttpService httpService, ILocalSessionService sessionService) : IRoomRequestsService
    {
        /// <inheritdoc/>
        public async Task<RoomSession> CreateRoom(string roomName, string userName, CancellationToken ct = default)
        {
            var response = await httpService.PostAsync<RoomSession>("/api/v1/rooms", new { Name = roomName, UserName = userName }, ct);

            await sessionService.SaveAsync(response, ct);
            return response;
        }

        /// <inheritdoc/>
        public async Task<RoomSession> JoinRoom(string accessCode, string userName, CancellationToken ct = default)
        {
            var response = await httpService.PostAsync<RoomSession>("/api/v1/rooms/join", new { AccessCode = accessCode, UserName = userName }, ct);

            await sessionService.SaveAsync(response, ct);
            return response;
        }

        /// <inheritdoc/>
        public async Task DeleteRoom(string token, CancellationToken ct = default)
            => await httpService.DeleteAsync<object>("/api/v1/rooms", token, ct: ct);

        /// <inheritdoc/>
        public async Task<IReadOnlyList<RoomUserModel>> GetRoomUsers(string token, CancellationToken ct = default)
            => await httpService.GetAsync<IReadOnlyList<RoomUserModel>>("/api/v1/rooms", token, ct: ct);

        /// <inheritdoc/>
        public async Task<InviteCodeModel> GetInviteCodeRoom(string token, CancellationToken ct = default)
            => await httpService.GetAsync<InviteCodeModel>("/api/v1/rooms/invite-code", token, ct: ct);
    }
}