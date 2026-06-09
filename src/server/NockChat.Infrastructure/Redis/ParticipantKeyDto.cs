namespace NockChat.Infrastructure.Redis
{
    /// <summary>
    /// DTO для хранения данных участника в Redis
    /// Не содержит навигационных свойств EF — только данные
    /// </summary>
    internal sealed class ParticipantKeyDto
    {
        public int ChatUserId { get; set; }
        public int RoomId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string EphemeralPublicKey { get; set; } = string.Empty;
        public DateTimeOffset PublishedAt { get; set; }
    }
}