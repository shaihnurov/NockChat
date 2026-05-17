namespace NockChat.Domain.Entities
{
    public class ChatUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = [];
    }
}