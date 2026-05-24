using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Rooms;
using NockChat.Models.Sessions;

namespace NockChat.Services.HTTP.Requests.Rooms
{
    /// <summary>
    /// Интерфейс для HTTP-запросов, связанных с управлением комнатами
    /// </summary>
    public interface IRoomRequestsService
    {
        /// <summary>
        /// Создаёт новую комнату и сохраняет сессию локально
        /// </summary>
        /// <param name="roomName">Название комнаты</param>
        /// <param name="userName">Имя пользователя в комнате</param>
        Task<RoomSession> CreateRoom(string roomName, string userName, CancellationToken ct = default);

        /// <summary>
        /// Выполняет вход в существующую комнату по коду доступа и сохраняет сессию локально
        /// </summary>
        /// <param name="accessCode">Код доступа к комнате</param>
        /// <param name="userName">Имя пользователя в комнате</param>
        Task<RoomSession> JoinRoom(string accessCode, string userName, CancellationToken ct = default);

        /// <summary>
        /// Удаляет комнату на сервере
        /// </summary>
        /// <param name="token">Токен сессии комнаты</param>
        Task DeleteRoom(string token, CancellationToken ct = default);

        /// <summary>
        /// Возвращает список участников комнаты
        /// </summary>
        /// <param name="token">Токен сессии комнаты</param>
        Task<IReadOnlyList<RoomUserModel>> GetRoomUsers(string token, CancellationToken ct = default);

        /// <summary>
        /// Запрашивает код приглашения для входа в комнату
        /// </summary>
        /// <param name="token">Токен сессии комнаты</param>
        Task<InviteCodeModel> GetInviteCodeRoom(string token, CancellationToken ct = default);
    }
}