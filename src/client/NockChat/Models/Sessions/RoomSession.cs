using System;

namespace NockChat.Models.Sessions
{
    public class RoomSession
    {
        public string Token { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTimeOffset JoinedAt { get; set; }
    }
}