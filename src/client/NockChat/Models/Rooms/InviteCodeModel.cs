using System;

namespace NockChat.Models.Rooms
{
    public class InviteCodeModel
    {
        public string InviteCode { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}