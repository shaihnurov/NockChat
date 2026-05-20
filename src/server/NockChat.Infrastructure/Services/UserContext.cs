using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NockChat.Application.Common.Interfaces;

namespace NockChat.Infrastructure.Services
{
    /// <summary>
    /// Реализация <see cref="IUserContext"/>
    /// Извлекает данные пользователя из JWT-клеймов текущего HTTP-запроса
    /// </summary>
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        /// <inheritdoc/>
        public int ChatUserId => int.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("chatUserId"), out var v) ? v : 0;

        /// <inheritdoc/>
        public int RoomId => int.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("roomId"), out var v) ? v : 0;

        /// <inheritdoc/>
        public string? RoomName => httpContextAccessor.HttpContext?.User.FindFirstValue("roomName");

        /// <inheritdoc/>
        public string? Username => httpContextAccessor.HttpContext?.User.FindFirstValue("username");
    }
}