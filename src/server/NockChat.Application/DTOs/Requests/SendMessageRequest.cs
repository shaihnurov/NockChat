namespace NockChat.Application.DTOs.Requests
{
    /// <summary>
    /// Запрос на отправку сообщения в комнату чата
    /// </summary>
    /// <param name="ChatUserId">Идентификатор пользователя, отправляющего сообщение</param>
    /// <param name="Text">Текст сообщения</param>
    public record SendMessageRequest(int ChatUserId, string Text);
}