namespace NockChat.Application.DTOs.Requests
{
    public record SendMessageRequest(int ChatUserId, string Text);
}
