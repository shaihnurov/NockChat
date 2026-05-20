namespace NockChat.Domain.Entities
{
    /// <summary>
    /// Комната чата, объединяющая пользователей и их сообщения
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Уникальный идентификатор комнаты
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название комнаты
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Уникальный код доступа к комнате в формате <c>XXXX-XXXX</c>
        /// </summary>
        public string AccessCode { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время создания комнаты (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Коллекция сообщений в комнате
        /// </summary>
        public ICollection<Message> Messages { get; set; } = [];

        /// <summary>
        /// Коллекция пользователей, присоединившихся к комнате
        /// </summary>
        public ICollection<ChatUser> ChatUsers { get; set; } = [];
    }
}