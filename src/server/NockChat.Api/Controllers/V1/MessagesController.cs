using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NockChat.Application.Common.Pagination;
using NockChat.Application.DTOs.Responses;
using NockChat.Application.Messages.Queries;

namespace NockChat.Api.Controllers.V1
{
    /// <summary>
    /// Контроллер для управления сообщениями чата
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/messages")]
    public class MessagesController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Возвращает постраничный список сообщений
        /// </summary>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Количество сообщений на странице</param>
        /// <returns>Постраничный результат со списком сообщений</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<MessageResponse>>> GetMessages([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
            => Ok(await mediator.Send(new GetMessagesQuery(page, pageSize)));
    }
}