namespace NockChat.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(int chatUserId, int roomId, string roomName, string username);
    }
}