using Mapster;
using MapsterMapper;
using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Messages.Commands.SendMessage
{
    /// <summary>
    /// Команда для отправки сообщения в комнату чата
    /// </summary>
    /// <param name="RoomId">Идентификатор комнаты</param>
    /// <param name="ChatUserId">Идентификатор отправителя</param>
    /// <param name="Text">Текст сообщения</param>
    public record SendMessageCommand(int RoomId, int ChatUserId, string Text) : IRequest<MessageResponse>;

    /// <summary>
    /// Обработчик <see cref="SendMessageCommand"/>. Сохраняет сообщение в базе данных
    /// и рассылает его участникам комнаты через <see cref="IChatNotificationService"/>
    /// </summary>
    public class SendMessageCommandHandler(IMessageRepository messageRepository, IChatUserRepository chatUserRepository,
        IChatNotificationService chatNotification, IMapper mapper) : IRequestHandler<SendMessageCommand, MessageResponse>
    {
        /// <summary>
        /// Проверяет принадлежность пользователя к комнате, создаёт сообщение
        /// и отправляет уведомление остальным участникам
        /// </summary>
        /// <param name="request">Данные команды</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Созданное сообщение с флагом <c>IsOwn = true</c> для отправителя</returns>
        /// <exception cref="NotFoundException">Пользователь с указанным идентификатором не найден</exception>
        /// <exception cref="ConflictException">Пользователь не принадлежит указанной комнате</exception>
        public async Task<MessageResponse> Handle(SendMessageCommand request, CancellationToken ct)
        {
            var chatUser = await chatUserRepository.GetByIdAsync(request.ChatUserId, ct) ?? throw new NotFoundException("Пользователь не найден");

            var message = new Message
            {
                RoomId = request.RoomId,
                ChatUserId = request.ChatUserId,
                Text = request.Text,
                SentAt = DateTime.UtcNow
            };

            var created = await messageRepository.CreateAsync(message, ct);
            var response = (created, chatUser.Username).Adapt<MessageResponse>(mapper.Config);

            await chatNotification.SendMessageAsync(request.RoomId, response, ct);
            return response with { IsOwn = true };
        }
    }
}