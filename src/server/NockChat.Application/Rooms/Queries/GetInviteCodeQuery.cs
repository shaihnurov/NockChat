using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Rooms.Queries
{
    /// <summary>
    /// Запрос на получение временного кода приглашения в комнату
    /// </summary>
    public record GetInviteCodeQuery : IRequest<InviteCodeResponse>;

    /// <summary>
    /// Обработчик <see cref="GetInviteCodeQuery"/>
    /// </summary>
    public class GetInviteCodeHandler(IRoomRepository roomRepository, IUserContext userContext) : IRequestHandler<GetInviteCodeQuery, InviteCodeResponse>
    {
        private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(5);

        public async Task<InviteCodeResponse> Handle(GetInviteCodeQuery request, CancellationToken ct)
        {
            var roomId = userContext.RoomId;

            if (roomId == 0)
                throw new ForbiddenException("Токен не содержит информации о комнате");

            var room = await roomRepository.GetByIdAsync(roomId, ct) ?? throw new NotFoundException("Комната не найдена");

            if (room.InviteCode is not null && room.InviteCodeExpiresAt > DateTime.UtcNow)
                return new InviteCodeResponse(room.InviteCode, room.InviteCodeExpiresAt.Value);

            var newCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
            var expiresAt = DateTime.UtcNow.Add(CodeLifetime);

            await roomRepository.UpdateInviteCodeAsync(roomId, newCode, expiresAt, ct);
            return new InviteCodeResponse(newCode, expiresAt);
        }
    }
}