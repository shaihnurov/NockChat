using System;

namespace NockChat.Models.Rooms
{
    public class RoomModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}