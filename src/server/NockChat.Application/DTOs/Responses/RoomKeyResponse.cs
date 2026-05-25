namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Публичный ключ участника комнаты
    /// </summary>
    /// <param name="ChatUserId">Идентификатор участника</param>
    /// <param name="Username">Имя участника в комнате</param>
    /// <param name="EphemeralPublicKey">Публичный ключ Curve25519 в формате Base64</param>
    public record RoomKeyResponse(int ChatUserId, string Username, string EphemeralPublicKey);
}