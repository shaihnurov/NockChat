namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Ответ на успешный вход в комнату чата
    /// </summary>
    /// <param name="RoomId">ID созданной комнаты</param>
    /// <param name="RoomName">Название комнаты</param>
    /// <param name="Username">Имя вошедшего пользователя</param>
    /// <param name="Token">JWT-токен для доступа к комнате</param>
    public record JoinRoomResponse(int RoomId, string RoomName, string Username, string Token);
}