using System;

namespace NockChat.Models.Rooms
{
    /// <summary>
    /// Представляет участника комнаты чата
    /// </summary>
    public class RoomUserModel
    {
        /// <summary>
        /// Имя пользователя в рамках комнаты
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время вступления пользователя в комнату
        /// </summary>
        public DateTimeOffset JoinedAt { get; set; }
    }
}