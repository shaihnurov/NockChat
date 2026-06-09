using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Users.Commands.PublishKey
{
    /// <summary>
    /// Команда публикации ephemeral публичного ключа участника
    /// Вызывается клиентом сразу после подключения к SignalR
    /// </summary>
    /// <param name="EphemeralPublicKey">Публичный ключ Curve25519 в формате Base64</param>
    public record PublishKeyCommand(string EphemeralPublicKey) : IRequest<IReadOnlyList<RoomKeyResponse>>;

    /// <summary>
    /// Обработчик <see cref="PublishKeyCommand"/>
    /// </summary>
    public class PublishKeyCommandHandler(IParticipantKeyRepository keyRepository, IUserContext userContext,
        IChatCryptoService cryptoService) : IRequestHandler<PublishKeyCommand, IReadOnlyList<RoomKeyResponse>>
    {
        public async Task<IReadOnlyList<RoomKeyResponse>> Handle(PublishKeyCommand request, CancellationToken ct)
        {
            if (!cryptoService.IsValidPublicKey(request.EphemeralPublicKey))
                throw new ValidationException("Некорректный формат публичного ключа");

            await keyRepository.UpsertAsync(new ParticipantKey
            {
                ChatUserId = userContext.ChatUserId,
                RoomId = userContext.RoomId,
                EphemeralPublicKey = request.EphemeralPublicKey,
                PublishedAt = DateTimeOffset.UtcNow
            }, ct);

            var existingKeys = await keyRepository.GetRoomKeysAsync(userContext.RoomId, userContext.ChatUserId, ct);

            return [.. existingKeys.Select(k => new RoomKeyResponse(k.ChatUserId, k.ChatUser.Username, k.EphemeralPublicKey))];
        }
    }
}