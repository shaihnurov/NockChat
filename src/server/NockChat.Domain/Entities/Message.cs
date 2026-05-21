namespace NockChat.Domain.Entities
{
    /// <summary>
    /// Сообщение, отправленное пользователем в комнате чата
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Уникальный идентификатор сообщения
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время отправки сообщения (UTC)
        /// </summary>
        public DateTimeOffset SentAt { get; set; }

        /// <summary>
        /// Идентификатор комнаты, в которой отправлено сообщение
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Навигационное свойство комнаты
        /// </summary>
        public Room Room { get; set; } = null!;

        /// <summary>
        /// Идентификатор пользователя, отправившего сообщение
        /// </summary>
        public int ChatUserId { get; set; }

        /// <summary>
        /// Навигационное свойство пользователя
        /// </summary>
        public ChatUser ChatUser { get; set; } = null!;
    }
}