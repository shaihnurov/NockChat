namespace NockChat.Application.DTOs.Responses
{
    public record CreateRoomResponse(string RoomName, string AccessCode, string Username, string Token);
}