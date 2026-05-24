using System;

namespace NockChat.Models.Messages
{
    /// <summary>
    /// Представляет сообщение в чате
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Имя пользователя, отправившего сообщение
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Указывает, является ли сообщение отправленным текущим пользователем
        /// </summary>
        public bool IsOwn { get; set; }

        /// <summary>
        /// Дата и время отправки сообщения
        /// </summary>
        public DateTimeOffset SentAt { get; set; }
    }
}