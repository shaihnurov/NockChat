using System;

namespace NockChat.Models.Messages
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsOwn { get; set; }
        public DateTimeOffset SentAt { get; set; }
    }
}