namespace NockChat.Application.DTOs.Responses
{
    public record JoinRoomResponse(int RoomId, string RoomName, int ChatUserId, string Username);
}