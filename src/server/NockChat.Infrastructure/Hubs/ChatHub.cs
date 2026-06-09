using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Requests;
using NockChat.Application.DTOs.Responses;
using NockChat.Application.Messages.Commands.SendMessage;
using NockChat.Application.Users.Commands.PublishKey;

namespace NockChat.Infrastructure.Hubs
{
    /// <summary>
    /// SignalR-хаб для обмена сообщениями в комнате чата в режиме реального времени
    /// Требует авторизации: идентификаторы пользователя и комнаты извлекаются из JWT-клеймов
    /// </summary>
    [Authorize]
    public class ChatHub(IMediator mediator, IParticipantKeyRepository keyRepository) : Hub<IChatHubClient>
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
            await keyRepository.DeleteAsync(ChatUserId, RoomId);
            await Clients.OthersInGroup(RoomId.ToString()).UserLeft(Username);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Принимает текстовое сообщение от клиента и передаёт его в обработчик через MediatR
        /// </summary>
        /// <param name="text">Текст отправляемого сообщения</param>
        public async Task SendMessage(EncryptedPayloadRequest payload)
            => await mediator.Send(new SendMessageCommand(RoomId, ChatUserId, payload, Context.ConnectionId));

        /// <summary>
        /// Клиент публикует свой ephemeral публичный ключ сразу после подключения
        /// В ответ получает ключи всех остальных участников комнаты
        /// </summary>
        /// <param name="ephemeralPublicKey">Публичный ключ Curve25519 в формате Base64</param>
        public async Task PublishKey(string ephemeralPublicKey)
        {
            var roomKeys = await mediator.Send(new PublishKeyCommand(ephemeralPublicKey));

            await Clients.Caller.ReceiveRoomKeys(roomKeys);
            await Clients.OthersInGroup(RoomId.ToString()).ParticipantKeyPublished(new RoomKeyResponse(ChatUserId, Username, ephemeralPublicKey));

            await Clients.OthersInGroup(RoomId.ToString()).UserJoined(Username);
        }
    }
}