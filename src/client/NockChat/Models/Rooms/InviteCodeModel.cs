using System;

namespace NockChat.Models.Rooms
{
    /// <summary>
    /// Представляет код приглашения для входа в комнату чата
    /// </summary>
    public class InviteCodeModel
    {
        /// <summary>
        /// Строковый код приглашения, который передаётся другому пользователю для входа в комнату
        /// </summary>
        public string InviteCode { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время истечения срока действия кода приглашения
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }
    }
}