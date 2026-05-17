using MediatR;
using NockChat.Application.Common.Interfaces;
using NockChat.Application.DTOs.Responses;
using NockChat.Domain.Entities;

namespace NockChat.Application.Rooms.Commands.CreateRoom
{
    public record CreateRoomCommand(string Name) : IRequest<RoomResponse>;

    public class CreateRoomCommandHandler(IRoomRepository roomRepository) : IRequestHandler<CreateRoomCommand, RoomResponse>
    {
        public async Task<RoomResponse> Handle(CreateRoomCommand request, CancellationToken ct)
        {
            var room = new Room
            {
                Name = request.Name,
                AccessCode = GenerateAccessCode(),
                CreatedAt = DateTime.UtcNow
            };

            var created = await roomRepository.CreateAsync(room, ct);

            return new RoomResponse(
                created.Id,
                created.Name,
                created.AccessCode,
                created.CreatedAt
            );
        }

        private static string GenerateAccessCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            var part1 = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
            var part2 = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
            return $"{part1}-{part2}";
        }
    }
}