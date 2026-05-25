namespace NockChat.Application.DTOs.Requests
{
    /// <summary>
    /// Запрос на публикацию ephemeral публичного ключа участника
    /// </summary>
    /// <param name="EphemeralPublicKey">Публичный ключ Curve25519 в формате Base64</param>
    public record PublishKeyRequest(string EphemeralPublicKey);
}