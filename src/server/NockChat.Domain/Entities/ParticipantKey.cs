namespace NockChat.Domain.Entities
{
    /// <summary>
    /// Ephemeral публичный ключ участника комнаты для E2E шифрования
    /// Приватный ключ хранится только на клиенте и никогда не передаётся на сервер
    /// </summary>
    public class ParticipantKey
    {
        /// <summary>
        /// Уникальный идентификатор записи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ephemeral публичный ключ Curve25519 в формате Base64
        /// Генерируется клиентом при каждом входе в комнату
        /// </summary>
        public string EphemeralPublicKey { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время публикации ключа (UTC)
        /// </summary>
        public DateTimeOffset PublishedAt { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому принадлежит ключ
        /// </summary>
        public int ChatUserId { get; set; }

        /// <summary>
        /// Навигационное свойство пользователя
        /// </summary>
        public ChatUser ChatUser { get; set; } = null!;

        /// <summary>
        /// Идентификатор комнаты
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Навигационное свойство комнаты
        /// </summary>
        public Room Room { get; set; } = null!;
    }
}