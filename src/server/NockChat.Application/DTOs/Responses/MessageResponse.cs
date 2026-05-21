namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Данные сообщения чата для передачи клиенту
    /// </summary>
    /// <param name="Id">Идентификатор сообщения</param>
    /// <param name="Text">Текст сообщения</param>
    /// <param name="Username">Имя автора сообщения</param>
    /// <param name="IsOwn"><c>true</c>, если сообщение отправлено текущим пользователем</param>
    /// <param name="SentAt">Дата и время отправки сообщения (UTC)</param>
    public record MessageResponse(int Id, string Text, string Username, bool IsOwn, DateTimeOffset SentAt);
}