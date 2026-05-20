using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NockChat.Application.DTOs.Requests;
using NockChat.Application.DTOs.Responses;
using NockChat.Application.Rooms.Commands.CreateRoom;
using NockChat.Application.Rooms.Commands.JoinRoom;

namespace NockChat.Api.Controllers.V1
{
    /// <summary>
    /// Контроллер для управления комнатами чата.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/rooms")]
    public class RoomsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Создаёт новую комнату чата и возвращает код доступа к ней
        /// </summary>
        /// <param name="request">Данные для создания комнаты: название и имя пользователя</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Созданная комната с кодом доступа</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateRoomResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new CreateRoomCommand(request.Name, request.Username), ct);
            return CreatedAtAction(nameof(CreateRoom), new { result.AccessCode }, result);
        }

        /// <summary>
        /// Присоединяет пользователя к существующей комнате по коду доступа
        /// </summary>
        /// <param name="request">Данные для входа в комнату: код доступа и имя пользователя</param>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns>Данные комнаты при успешном подключении</returns>
        [HttpPost("join")]
        [AllowAnonymous]
        [EnableRateLimiting("join-room")]
        [ProducesResponseType(typeof(CreateRoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest request, CancellationToken ct)
            => Ok(await mediator.Send(new JoinRoomCommand(request.AccessCode, request.Username), ct));
    }
}