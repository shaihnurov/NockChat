namespace NockChat.Application.DTOs.Requests
{
    /// <summary>
    /// Зашифрованный payload входящего сообщения от клиента
    /// </summary>
    public record EncryptedPayloadRequest(string Nonce, string Ciphertext, string RatchetPublicKey, int Counter);
}