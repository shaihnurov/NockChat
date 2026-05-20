using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для отправки уведомлений участникам комнаты чата через SignalR
    /// </summary>
    public interface IChatNotificationService
    {
        /// <summary>
        /// Рассылает новое сообщение всем участникам указанной комнаты
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="message">Данные отправляемого сообщения</param>
        /// <param name="ct">Токен</param>
        Task SendMessageAsync(int roomId, MessageResponse message, CancellationToken ct = default);

        /// <summary>
        /// Уведомляет участников комнаты о подключении нового пользователя
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="username">Имя подключившегося пользователя</param>
        /// <param name="ct">Токен</param>
        Task NotifyUserJoinedAsync(int roomId, string username, CancellationToken ct = default);

        /// <summary>
        /// Уведомляет участников комнаты об отключении пользователя
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="username">Имя отключившегося пользователя</param>
        /// <param name="ct">Токен</param>
        Task NotifyUserLeftAsync(int roomId, string username, CancellationToken ct = default);
    }
}