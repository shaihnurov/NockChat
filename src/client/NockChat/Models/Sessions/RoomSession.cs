using System;

namespace NockChat.Models.Sessions
{
    /// <summary>
    /// Представляет сессию пользователя в комнате чата
    /// </summary>
    public class RoomSession
    {
        /// <summary>
        /// Токен сессии для идентификации пользователя в рамках комнаты
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Название комнаты, к которой привязана сессия
        /// </summary>
        public string RoomName { get; set; } = string.Empty;

        /// <summary>
        /// Имя пользователя в рамках данной комнаты
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время входа пользователя в комнату
        /// </summary>
        public DateTimeOffset JoinedAt { get; set; }
    }
}