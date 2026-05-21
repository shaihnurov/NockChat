namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Ответ с временным кодом приглашения в комнату
    /// </summary>
    /// <param name="InviteCode">Временный код</param>
    /// <param name="ExpiresAt">Время истечения кода</param>
    public record InviteCodeResponse(string InviteCode, DateTimeOffset ExpiresAt);
}