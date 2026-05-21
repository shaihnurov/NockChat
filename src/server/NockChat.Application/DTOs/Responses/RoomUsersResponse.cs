namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Ответ с данными участника комнаты
    /// </summary>
    /// <param name="Username">Имя пользователя</param>
    /// <param name="JoinedAt">Дата и время вступления в комнату</param>
    public record RoomUsersResponse(string Username, DateTimeOffset JoinedAt);
}