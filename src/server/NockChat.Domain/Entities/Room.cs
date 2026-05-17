namespace NockChat.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccessCode { get; set; } = GenerateCode();
        public DateTime CreatedAt { get; set; }

        public ICollection<Message> Messages { get; set; } = [];
        public ICollection<ChatUser> ChatUsers { get; set; } = [];

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            var part1 = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
            var part2 = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
            return $"{part1}-{part2}";
        }
    }
}