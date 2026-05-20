using Microsoft.AspNetCore.SignalR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Infrastructure.Hubs;

namespace NockChat.Infrastructure.Services
{
    /// <summary>
    /// Реализация <see cref="IChatNotificationService"/> на основе SignalR
    /// Рассылает уведомления через <see cref="IHubContext{THub, T}"/> в группу комнаты
    /// </summary>
    public class ChatNotificationService(IHubContext<ChatHub, IChatHubClient> hubContext) : IChatNotificationService
    {
        /// <inheritdoc/>
        public async Task SendMessageAsync(int roomId, MessageResponse message, CancellationToken ct = default)
            => await hubContext.Clients.Group(roomId.ToString()).ReceiveMessage(message);

        /// <inheritdoc/>
        public async Task NotifyUserJoinedAsync(int roomId, string username, CancellationToken ct = default)
            => await hubContext.Clients.Group(roomId.ToString()).UserJoined(username);

        /// <inheritdoc/>
        public async Task NotifyUserLeftAsync(int roomId, string username, CancellationToken ct = default)
            => await hubContext.Clients.Group(roomId.ToString()).UserLeft(username);
    }
}