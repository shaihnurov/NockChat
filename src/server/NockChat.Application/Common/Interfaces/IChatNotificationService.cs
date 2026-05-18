using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Common.Interfaces
{
    public interface IChatNotificationService
    {
        Task SendMessageAsync(int roomId, MessageResponse message, CancellationToken ct = default);
        Task NotifyUserJoinedAsync(int roomId, string username, CancellationToken ct = default);
        Task NotifyUserLeftAsync(int roomId, string username, CancellationToken ct = default);
    }
}