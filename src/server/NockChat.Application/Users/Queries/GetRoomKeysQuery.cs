using MediatR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Users.Queries
{
    /// <summary>
    /// Запрос на получение публичных ключей всех участников комнаты
    /// </summary>
    public record GetRoomKeysQuery : IRequest<IReadOnlyList<RoomKeyResponse>>;

    /// <summary>
    /// Обработчик <see cref="GetRoomKeysQuery"/>
    /// </summary>
    public class GetRoomKeysQueryHandler(IParticipantKeyRepository keyRepository, IUserContext userContext) : IRequestHandler<GetRoomKeysQuery, IReadOnlyList<RoomKeyResponse>>
    {
        public async Task<IReadOnlyList<RoomKeyResponse>> Handle(GetRoomKeysQuery request, CancellationToken ct)
        {
            var keys = await keyRepository.GetRoomKeysAsync(userContext.RoomId, userContext.ChatUserId, ct);

            return [.. keys.Select(k => new RoomKeyResponse(k.ChatUserId, k.ChatUser.Username, k.EphemeralPublicKey))];
        }
    }
}