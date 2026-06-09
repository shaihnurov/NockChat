namespace NockChat.Application.DTOs.Responses
{
    /// <summary>
    /// Данные зашифрованного сообщения для передачи клиенту
    /// Сервер не знает содержимого — только метаданные и зашифрованный blob
    /// </summary>
    /// <param name="Id">Идентификатор сообщения</param>
    /// <param name="SenderId">Идентификатор отправителя — клиент ищет по нему крипто-сессию</param>
    /// <param name="Username">Имя отправителя</param>
    /// <param name="EncryptedPayload">Зашифрованный payload — Nonce, Ciphertext, RatchetPublicKey, Counter</param>
    /// <param name="IsOwn">true если сообщение отправлено текущим пользователем</param>
    /// <param name="SentAt">Дата и время отправки (UTC)</param>
    public record MessageResponse(int Id, int SenderId, string Username, EncryptedPayloadResponse EncryptedPayload, bool IsOwn, DateTimeOffset SentAt);
}