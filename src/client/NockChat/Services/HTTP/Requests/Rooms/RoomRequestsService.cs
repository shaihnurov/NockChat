using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NockChat.Models.Rooms;
using NockChat.Models.Sessions;
using NockChat.Services.Common.DataStorage.Sessions;
using NockChat.ViewModels;

namespace NockChat.Services.HTTP.Requests.Rooms
{
    public class RoomRequestsService(ILogger<HomeViewModel> logger, IHttpService httpService, ILocalSessionService sessionService) : IRoomRequestsService
    {
        public async Task<RoomSession?> CreateRoom(string roomName, string userName, CancellationToken ct = default)
        {
            var (success, response, error) = await httpService.PostAsync<RoomSession>("/api/v1/rooms", new { Name = roomName, UserName = userName }, ct);

            if (!success || response == null)
            {
                logger.LogWarning("Ошибка при создании комнаты: {Error}", error);
                return null;
            }

            var session = new RoomSession
            {
                Token = response.Token,
                RoomName = response.RoomName,
                Username = response.Username,
                JoinedAt = DateTime.UtcNow
            };

            await sessionService.SaveAsync(session, ct);
            return session;
        }

        public async Task<RoomSession?> JoinRoom(string accessCode, string userName, CancellationToken ct = default)
        {
            var (success, response, error) = await httpService.PostAsync<RoomSession>("/api/v1/rooms/join", new { AccessCode = accessCode, UserName = userName }, ct);

            if (!success || response == null)
            {
                logger.LogWarning("Ошибка при входе в комнату: {Error}", error);
                return null;
            }

            var session = new RoomSession
            {
                Token = response.Token,
                RoomName = response.RoomName,
                Username = response.Username,
                JoinedAt = DateTime.UtcNow
            };

            await sessionService.SaveAsync(session, ct);
            return session;
        }

        public async Task DeleteRoom(string token, CancellationToken ct = default)
        {
            var (success, _, error) = await httpService.DeleteAsync<object>("/api/v1/rooms", token, ct: ct);

            if (!success)
            {
                logger.LogWarning("Ошибка при удалении комнаты: {Error}", error);
                return;
            }
        }

        public async Task<IReadOnlyList<RoomUserModel>?> GetRoomUsers(string token, CancellationToken ct = default)
        {
            var (success, response, error) = await httpService.GetAsync<IReadOnlyList<RoomUserModel>>("/api/v1/rooms", token, ct: ct);

            if (!success || response == null)
            {
                logger.LogWarning("Ошибка при получении пользователей комнаты: {Error}", error);
                return null;
            }

            return response;
        }
    }
}