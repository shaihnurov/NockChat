using MediatR;
using NockChat.Application.Common.Exceptions;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Rooms.Commands.JoinRoom
{
    public record JoinRoomCommand(string AccessCode, string Username) : IRequest<JoinRoomResponse>;

    public class JoinRoomCommandHandler(IRoomRepository roomRepository, IChatUserRepository chatUserRepository, 
        ITokenService tokenService) : IRequestHandler<JoinRoomCommand, JoinRoomResponse>
    {
        public async Task<JoinRoomResponse> Handle(JoinRoomCommand request, CancellationToken ct)
        {
            var room = await roomRepository.GetByAccessCodeAsync(request.AccessCode, ct) ?? throw new NotFoundException($"Комната не найдена");

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