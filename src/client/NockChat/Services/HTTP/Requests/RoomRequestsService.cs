using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NockChat.Models.Rooms;
using NockChat.ViewModels;

namespace NockChat.Services.HTTP.Requests
{
    public class RoomRequestsService(ILogger<HomeViewModel> logger, IHttpService httpService) : IRoomRequestsService
    {
        public async Task<RoomModel?> CreateRoom(string roomName, CancellationToken ct)
        {
            var (success, response, error) = await httpService.PostAsync<RoomModel>("/api/v1/rooms", new { Name = roomName }, ct);

            if (!success)
            {
                logger.LogWarning("Возникла ошибка при создании комнаты: {Error}", error);
                return null;
            }

            if (response == null)
            {
                logger.LogWarning("Не удалось получить информацию о комнате");
                return null;
            }

            return response;
        }
    }
}