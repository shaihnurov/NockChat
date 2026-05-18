using MediatR;
using Microsoft.AspNetCore.SignalR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.Messages.Commands.SendMessage;

namespace NockChat.Infrastructure.Hubs
{
    public class ChatHub(IMediator mediator, ITokenService tokenService) : Hub<IChatHubClient>
    {
        public async Task JoinRoom(string token)
        {
            var (chatUserId, roomId) = tokenService.ValidateToken(token) ?? throw new HubException("Недействительный токен");

            Context.Items["chatUserId"] = chatUserId;
            Context.Items["roomId"] = roomId;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        public async Task LeaveRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());

            var username = Context.Items["username"]?.ToString() ?? "Unknown";
            await Clients.OthersInGroup(roomId.ToString()).UserLeft(username);
        }

        public async Task SendMessage(string token, string text)
        {
            var (chatUserId, roomId) = tokenService.ValidateToken(token) ?? throw new HubException("Недействительный токен");

            await mediator.Send(new SendMessageCommand(roomId, chatUserId, text));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}