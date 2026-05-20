namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Ответ на успешный вход в комнату чата
    /// </summary>
    /// <param name="RoomName">Название комнаты</param>
    /// <param name="Username">Имя вошедшего пользователя</param>
    /// <param name="Token">JWT-токен для доступа к комнате</param>
    public record JoinRoomResponse(string RoomName, string Username, string Token);
}