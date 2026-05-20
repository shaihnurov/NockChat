namespace NockChat.Application.DTOs.Requests
{
    /// <summary>
    /// Запрос на создание новой комнаты чата
    /// </summary>
    /// <param name="Name">Название комнаты</param>
    /// <param name="Username">Имя пользователя — создателя комнаты</param>
    public record CreateRoomRequest(string Name, string Username);
}