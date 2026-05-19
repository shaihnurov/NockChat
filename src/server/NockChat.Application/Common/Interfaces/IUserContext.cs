namespace NockChat.Application.Common.Interfaces
{
    public interface IUserContext
    {
        public int ChatUserId { get; }
        public int RoomId { get; }
        public string? RoomName { get; }
        public string? Username { get; }
    }
}
