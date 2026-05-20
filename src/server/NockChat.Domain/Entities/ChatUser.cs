namespace NockChat.Domain.Entities
{
    /// <summary>
    /// Пользователь чата, привязанный к конкретной комнате
    /// </summary>
    public class ChatUser
    {
        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя в комнате
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время входа пользователя в комнату (UTC)
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Идентификатор комнаты, к которой принадлежит пользователь
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Навигационное свойство комнаты
        /// </summary>
        public Room Room { get; set; } = null!;

        /// <summary>
        /// Коллекция сообщений, отправленных пользователем
        /// </summary>
        public ICollection<Message> Messages { get; set; } = [];
    }
}