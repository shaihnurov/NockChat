using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;

namespace NockChat.Application.Rooms.Commands.DeleteRoom
{
    /// <summary>
    /// Команда для удаления существующей комнаты чата
    /// </summary>
    public record DeleteRoomCommand : IRequest;

    /// <summary>
    /// Обработчик <see cref="DeleteRoomCommand"/>
    /// </summary>
    public class DeleteRoomCommandHandler(IRoomRepository roomRepository, IUserContext userContext) : IRequestHandler<DeleteRoomCommand>
    {
        /// <summary>
        /// Выполняет удаление комнаты
        /// </summary>
        /// <param name="request">Данные команды</param>
        /// <param name="ct">Токен</param>
        public async Task Handle(DeleteRoomCommand request, CancellationToken ct)
        {
            var roomId = userContext.RoomId;

            if (roomId == 0)
                throw new ForbiddenException("Токен не содержит информации о комнате");

            await roomRepository.DeleteAsync(roomId, ct);
        }
    }
}