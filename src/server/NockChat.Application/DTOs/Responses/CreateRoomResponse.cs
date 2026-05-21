namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Ответ на успешное создание комнаты чата
    /// </summary>
    /// <param name="RoomName">Название созданной комнаты</param>
    /// <param name="Username">Имя пользователя — создателя комнаты</param>
    /// <param name="Token">JWT-токен для доступа к комнате</param>
    public record CreateRoomResponse(string RoomName, string Username, string Token);
}