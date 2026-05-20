using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.Messages.Commands.SendMessage;

namespace NockChat.Infrastructure.Hubs
{
    /// <summary>
    /// SignalR-хаб для обмена сообщениями в комнате чата в режиме реального времени
    /// Требует авторизации: идентификаторы пользователя и комнаты извлекаются из JWT-клеймов
    /// </summary>
    [Authorize]
    public class ChatHub(IMediator mediator) : Hub<IChatHubClient>
    {
        private int ChatUserId => int.Parse(Context.User!.FindFirstValue("chatUserId")!);
        private int RoomId => int.Parse(Context.User!.FindFirstValue("roomId")!);
        private string Username => Context.User!.FindFirstValue("username") ?? "Unknown";

        /// <summary>
        /// Вызывается при подключении клиента. Добавляет соединение в группу комнаты
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, RoomId.ToString());
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Вызывается при отключении клиента. Уведомляет остальных участников комнаты об уходе пользователя
        /// </summary>
        /// <param name="exception">Исключение, ставшее причиной разрыва соединения, или <c>null</c> при штатном отключении</param>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.OthersInGroup(RoomId.ToString()).UserLeft(Username);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Принимает текстовое сообщение от клиента и передаёт его в обработчик через MediatR
        /// </summary>
        /// <param name="text">Текст отправляемого сообщения</param>
        public async Task SendMessage(string text)
            => await mediator.Send(new SendMessageCommand(RoomId, ChatUserId, text));
    }
}