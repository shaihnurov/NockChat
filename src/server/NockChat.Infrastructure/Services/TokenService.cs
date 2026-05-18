using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NockChat.Application.Common.Interfaces;

namespace NockChat.Infrastructure.Services
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
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

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(30), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (int chatUserId, int roomId)? ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out var validatedToken);

                var jwt = (JwtSecurityToken)validatedToken;
                var chatUserId = int.Parse(jwt.Claims.First(c => c.Type == "chatUserId").Value);
                var roomId = int.Parse(jwt.Claims.First(c => c.Type == "roomId").Value);

                return (chatUserId, roomId);
            }
            catch
            {
                return null;
            }
        }
    }
}
