namespace NockChat.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public int ChatUserId { get; set; }
        public ChatUser ChatUser { get; set; } = null!;
    }
}