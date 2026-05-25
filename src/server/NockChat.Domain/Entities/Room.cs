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
        /// Дата и время создания комнаты (UTC)
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Временный код приглашения для доступа в комнату, действующий до <see cref="InviteCodeExpiresAt"/>
        /// </summary>
        public string? InviteCode { get; set; }

        /// <summary>
        /// Время действия кода приглашения
        /// </summary>
        public DateTimeOffset? InviteCodeExpiresAt { get; set; }

        /// <summary>
        /// Коллекция сообщений в комнате
        /// </summary>
        public ICollection<Message> Messages { get; set; } = [];

        /// <summary>
        /// Коллекция пользователей, присоединившихся к комнате
        /// </summary>
        public ICollection<ChatUser> ChatUsers { get; set; } = [];

        /// <summary>
        /// Коллекция публичных ключей участников комнаты
        /// </summary>
        public ICollection<ParticipantKey> ParticipantKeys { get; set; } = [];
    }
}