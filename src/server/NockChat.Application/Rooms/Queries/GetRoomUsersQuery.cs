using Mapster;
using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;

namespace NockChat.Application.Rooms.Queries
{
    /// <summary>
    /// Запрос на получение списка участников чата
    /// </summary>
    public record GetRoomUsersQuery : IRequest<IReadOnlyList<RoomUsersResponse>>;

    /// <summary>
    /// Обработчик <see cref="GetRoomUsersQuery"/>
    /// </summary>
    public class GetRoomUsersHandler(IChatUserRepository chatUserRepository, IUserContext userContext) : IRequestHandler<GetRoomUsersQuery, IReadOnlyList<RoomUsersResponse>>
    {
        /// <summary>
        /// Загружает всех пользователей чата
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <param name="cancellationToken">Токен</param>
        /// <returns>Список пользователей</returns>
        public async Task<IReadOnlyList<RoomUsersResponse>> Handle(GetRoomUsersQuery request, CancellationToken cancellationToken)
        {
            var roomId = userContext.RoomId;

            if (roomId == 0)
                throw new ForbiddenException("Токен не содержит информации о комнате");

            var users = await chatUserRepository.GetRoomUsersAsync(roomId, cancellationToken);
            return users.Adapt<IReadOnlyList<RoomUsersResponse>>();
        }
    }
}