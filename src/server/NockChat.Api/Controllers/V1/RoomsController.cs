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
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/rooms")]
    public class RoomsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateRoomResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new CreateRoomCommand(request.Name, request.Username), ct);
            return CreatedAtAction(nameof(CreateRoom), new { result.AccessCode }, result);
        }

        [HttpPost("join")]
        [AllowAnonymous]
        [EnableRateLimiting("join-room")]
        [ProducesResponseType(typeof(CreateRoomResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new JoinRoomCommand(request.AccessCode, request.Username), ct);
            return Ok(result);
        }
    }
}