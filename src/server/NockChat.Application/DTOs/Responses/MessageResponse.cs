namespace NockChat.Application.DTOs.Responses
{
    public record MessageResponse(int Id, string Text, string Username, DateTime SentAt);
}