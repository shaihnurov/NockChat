using Mapster;
using MapsterMapper;
using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Messages.Commands.SendMessage
{
    public record SendMessageCommand(int RoomId, int ChatUserId, string Text) : IRequest<MessageResponse>;

    public class SendMessageCommandHandler(IMessageRepository messageRepository, IChatUserRepository chatUserRepository,
        IChatNotificationService chatNotification, IMapper mapper) : IRequestHandler<SendMessageCommand, MessageResponse>
    {
        public async Task<MessageResponse> Handle(SendMessageCommand request, CancellationToken ct)
        {
            var chatUser = await chatUserRepository.GetByIdAsync(request.ChatUserId, ct) ?? throw new NotFoundException("Пользователь не найден");

            if (chatUser.RoomId != request.RoomId)
                throw new ConflictException("Пользователь не принадлежит этой комнате");

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