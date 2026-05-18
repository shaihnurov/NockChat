using Microsoft.AspNetCore.SignalR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Infrastructure.Hubs;

namespace NockChat.Infrastructure.Services
{
    public class ChatNotificationService(IHubContext<ChatHub, IChatHubClient> hubContext) : IChatNotificationService
    {
        public async Task SendMessageAsync(int roomId, MessageResponse message, CancellationToken ct = default)
            => await hubContext.Clients.Group(roomId.ToString()).ReceiveMessage(message);

        public async Task NotifyUserJoinedAsync(int roomId, string username, CancellationToken ct = default)
            => await hubContext.Clients.Group(roomId.ToString()).UserJoined(username);

        public async Task NotifyUserLeftAsync(int roomId, string username, CancellationToken ct = default)
            => await hubContext.Clients.Group(roomId.ToString()).UserLeft(username);
    }
}