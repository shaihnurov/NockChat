namespace NockChat.Models.Rooms
{
    /// <summary>
    /// Публичный ключ участника комнаты, полученный с сервера
    /// </summary>
    public sealed class RoomKeyModel
    {
        /// <summary>
        /// Идентификатор участника комнаты, которому принадлежит этот ключ
        /// </summary>
        public int ChatUserId { get; set; }

        /// <summary>
        /// Имя участника комнаты, которому принадлежит этот ключ
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Публичный ключ Curve25519 в формате Base64
        /// </summary>
        public string EphemeralPublicKey { get; set; } = string.Empty;
    }
}