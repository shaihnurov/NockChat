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
                AccessCode = GenerateAccessCode(),
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

            return new CreateRoomResponse(createdRoom.Name, createdRoom.AccessCode, createdUser.Username, token);
        }

        /// <summary>
        /// Генерирует случайный код доступа к комнате в формате <c>XXXX-XXXX</c>
        /// из символов верхнего регистра и цифр (без визуально схожих знаков)
        /// </summary>
        /// <returns>Код доступа в формате <c>XXXX-XXXX</c></returns>
        private static string GenerateAccessCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            var part1 = new string([.. Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)])]);
            var part2 = new string([.. Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)])]);
            return $"{part1}-{part2}";
        }
    }
}