using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Rooms.Commands.JoinRoom
{
    /// <summary>
    /// Команда для входа пользователя в существующую комнату чата
    /// </summary>
    /// <param name="InviteCode">Код доступа к комнате</param>
    /// <param name="Username">Имя пользователя, входящего в комнату</param>
    public record JoinRoomCommand(string InviteCode, string Username) : IRequest<JoinRoomResponse>;

    /// <summary>
    /// Обработчик <see cref="JoinRoomCommand"/>
    /// </summary>
    public class JoinRoomCommandHandler(IRoomRepository roomRepository, IChatUserRepository chatUserRepository,
        ITokenService tokenService) : IRequestHandler<JoinRoomCommand, JoinRoomResponse>
    {
        /// <summary>
        /// Выполняет вход пользователя в комнату по коду доступа
        /// </summary>
        /// <param name="request">Данные команды</param>
        /// <param name="ct">Токен</param>
        /// <returns>Данные комнаты и JWT-токен для доступа</returns>
        /// <exception cref="NotFoundException">Комната с указанным кодом доступа не найдена</exception>
        /// <exception cref="ConflictException">Имя пользователя уже занято в этой комнате</exception>
        public async Task<JoinRoomResponse> Handle(JoinRoomCommand request, CancellationToken ct)
        {
            var room = await roomRepository.GetByInviteCodeAsync(request.InviteCode, ct) ?? throw new NotFoundException("Комната не найдена или код истёк");

            var userExists = await chatUserRepository.ExistsAsync(room.Id, request.Username, ct);
            if (userExists)
                throw new ConflictException($"Имя '{request.Username}' уже занято в этой комнате");

            var chatUser = new ChatUser
            {
                Username = request.Username,
                RoomId = room.Id,
                JoinedAt = DateTime.UtcNow
            };

            var created = await chatUserRepository.CreateAsync(chatUser, ct);

            var token = tokenService.GenerateToken(created.Id, room.Id, room.Name, created.Username);

            return new JoinRoomResponse(room.Name, created.Username, token);
        }
    }
}