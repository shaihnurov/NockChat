using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Интерфейс клиента SignalR-хаба для отправки событий чата подключённым пользователям
    /// </summary>
    public interface IChatHubClient
    {
        /// <summary>
        /// Отправляет новое сообщение всем слушателям группы
        /// </summary>
        /// <param name="message">Данные отправленного сообщения</param>
        Task ReceiveMessage(MessageResponse message);

        /// <summary>
        /// Уведомляет участников группы о подключении нового пользователя
        /// </summary>
        /// <param name="username">Имя подключившегося пользователя</param>
        Task UserJoined(string username);

        /// <summary>
        /// Уведомляет участников группы об отключении пользователя
        /// </summary>
        /// <param name="username">Имя отключившегося пользователя</param>
        Task UserLeft(string username);

        /// <summary>
        /// Отправляет вызывающему клиенту ключи всех участников, уже находящихся в комнате
        /// Вызывается единожды сразу после PublishKey — клиент устанавливает крипто-сессии
        /// </summary>
        Task ReceiveRoomKeys(IReadOnlyList<RoomKeyResponse> keys);

        /// <summary>
        /// Уведомляет участников комнаты о новом участнике и его публичном ключе
        /// Каждый получатель устанавливает крипто-сессию с новым участником
        /// </summary>
        Task ParticipantKeyPublished(RoomKeyResponse key);
    }
}