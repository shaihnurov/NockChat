namespace NockChat.Application.DTOs.Responses
{
    public record RoomResponse(int Id, string Name, string AccessCode, DateTime CreatedAt);
}