using MediatR;
using Microsoft.AspNetCore.SignalR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.Messages.Commands.SendMessage;

namespace NockChat.Infrastructure.Hubs
{
    public class ChatHub(IMediator mediator) : Hub<IChatHubClient>
    {
        public async Task JoinRoom(int roomId, int chatUserId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

            var username = Context.Items["username"]?.ToString() ?? "Unknown";
            await Clients.OthersInGroup(roomId.ToString()).UserJoined(username);
        }

        public async Task LeaveRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());

            var username = Context.Items["username"]?.ToString() ?? "Unknown";
            await Clients.OthersInGroup(roomId.ToString()).UserLeft(username);
        }

        public async Task SendMessage(int roomId, int chatUserId, string text)
        {
            await mediator.Send(new SendMessageCommand(roomId, chatUserId, text));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}