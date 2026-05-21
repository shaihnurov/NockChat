using System;

namespace NockChat.Models.Rooms
{
    public class RoomUserModel
    {
        public string Username { get; set; } = string.Empty;
        public DateTimeOffset JoinedAt { get; set; }
    }
}