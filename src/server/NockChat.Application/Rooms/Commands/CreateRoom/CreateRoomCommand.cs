using MediatR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Rooms.Commands.CreateRoom
{
    /// <summary>
    /// Команда для создания новой комнаты чата
    /// </summary>
    /// <param name="Name">Название комнаты</param>
    /// <param name="Username">Имя пользователя — создателя комнаты</param>
    public record CreateRoomCommand(string Name, string Username) : IRequest<CreateRoomResponse>;

    /// <summary>
    /// Обработчик <see cref="CreateRoomCommand"/>
    /// </summary>
    public class CreateRoomCommandHandler(IRoomRepository roomRepository, IChatUserRepository chatUserRepository,
        ITokenService tokenService) : IRequestHandler<CreateRoomCommand, CreateRoomResponse>
    {
        /// <summary>
        /// Создаёт комнату с уникальным кодом доступа, добавляет пользователя и возвращает токен
        /// </summary>
        /// <param name="request">Данные команды</param>
        /// <param name="ct">Токен</param>
        /// <returns>Данные созданной комнаты с кодом доступа и JWT-токеном</returns>
        public async Task<CreateRoomResponse> Handle(CreateRoomCommand request, CancellationToken ct)
        {
            var room = new Room
            {
                Name = request.Name,
                CreatedAt = DateTime.UtcNow
            };

            var createdRoom = await roomRepository.CreateAsync(room, ct);

            var chatUser = new ChatUser
            {
                Username = request.Username,
                RoomId = createdRoom.Id,
                JoinedAt = DateTime.UtcNow
            };

            var createdUser = await chatUserRepository.CreateAsync(chatUser, ct);

            var token = tokenService.GenerateToken(createdUser.Id, createdRoom.Id, createdRoom.Name, createdUser.Username);

            return new CreateRoomResponse(createdRoom.Name, createdUser.Username, token);
        }
    }
}