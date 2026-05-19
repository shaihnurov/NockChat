using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NockChat.Application.Common.Interfaces;

namespace NockChat.Infrastructure.Services
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public int ChatUserId => int.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("chatUserId"), out var v) ? v : 0;

        public int RoomId => int.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("roomId"), out var v) ? v : 0;

        public string? RoomName => httpContextAccessor.HttpContext?.User.FindFirstValue("roomName");
        public string? Username => httpContextAccessor.HttpContext?.User.FindFirstValue("username");
    }
}
