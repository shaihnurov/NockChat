namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Интерфейс для доступа к данным текущего аутентифицированного пользователя из контекста HTTP-запроса
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Идентификатор пользователя чата
        /// </summary>
        public int ChatUserId { get; }

        /// <summary>
        /// Идентификатор комнаты, к которой привязан пользователь
        /// </summary>
        public int RoomId { get; }

        /// <summary>
        /// Название комнаты или <c>null</c>, если клейм отсутствует
        /// </summary>
        public string? RoomName { get; }

        /// <summary>
        /// Имя пользователя или <c>null</c>, если клейм отсутствует
        /// </summary>
        public string? Username { get; }
    }
}