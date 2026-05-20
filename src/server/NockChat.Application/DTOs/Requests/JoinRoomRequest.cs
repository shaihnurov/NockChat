namespace NockChat.Application.DTOs.Requests
{
    /// <summary>
    /// Запрос на вход в существующую комнату чата
    /// </summary>
    /// <param name="AccessCode">Код доступа к комнате</param>
    /// <param name="Username">Имя пользователя, входящего в комнату</param>
    public record JoinRoomRequest(string AccessCode, string Username);
}