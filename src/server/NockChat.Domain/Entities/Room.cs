namespace NockChat.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public ICollection<Message> Messages { get; set; } = [];
        public ICollection<ChatUser> ChatUsers { get; set; } = [];
    }
}