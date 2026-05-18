using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Common.Interfaces
{
    public interface IChatHubClient
    {
        Task ReceiveMessage(MessageResponse message);
        Task UserJoined(string username);
        Task UserLeft(string username);
    }
}