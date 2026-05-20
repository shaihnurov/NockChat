using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NockChat.Application.Common.Interfaces;

namespace NockChat.Infrastructure.Services
{
    /// <summary>
    /// Реализация <see cref="ITokenService"/> на основе JWT
    /// </summary>
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        /// <inheritdoc/>
        public string GenerateToken(int chatUserId, int roomId, string roomName, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("chatUserId", chatUserId.ToString()),
                new Claim("roomId", roomId.ToString()),
                new Claim("roomName", roomName),
                new Claim("username", username)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}