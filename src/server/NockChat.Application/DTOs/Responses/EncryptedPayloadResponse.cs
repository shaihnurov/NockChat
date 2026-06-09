namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Зашифрованный payload сообщения
    /// Структура повторяет клиентский EncryptedMessage
    /// </summary>
    public record EncryptedPayloadResponse(string Nonce, string Ciphertext, string RatchetPublicKey, int Counter);
}