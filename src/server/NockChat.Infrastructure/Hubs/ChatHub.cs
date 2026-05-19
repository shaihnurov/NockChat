using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.Messages.Commands.SendMessage;

namespace NockChat.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub(IMediator mediator) : Hub<IChatHubClient>
    {
        private int ChatUserId => int.Parse(Context.User!.FindFirstValue("chatUserId")!);
        private int RoomId => int.Parse(Context.User!.FindFirstValue("roomId")!);
        private string Username => Context.User!.FindFirstValue("username") ?? "Unknown";

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, RoomId.ToString());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.OthersInGroup(RoomId.ToString()).UserLeft(Username);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string text)
            => await mediator.Send(new SendMessageCommand(RoomId, ChatUserId, text));
    }
}