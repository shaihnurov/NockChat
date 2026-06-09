namespace NockChat.Models.Crypto
{
    /// <summary>
    /// Зашифрованное сообщение передаваемое через сервер
    /// Сервер видит только этот объект — plaintext ему недоступен
    /// </summary>
    public sealed class EncryptedMessage
    {
        /// <summary>
        /// Одноразовый nonce для AES-GCM, 12 байт в Base64
        /// </summary>
        public required string Nonce { get; init; }

        /// <summary>
        /// Зашифрованный текст+16 байт GCM тег аутентификации в Base64
        /// </summary>
        public required string Ciphertext { get; init; }

        /// <summary>
        /// Текущий ephemeral публичный ключ отправителя в Base64
        /// Получатель сравнивает с предыдущим — если изменился выполняет DH ratchet шаг
        /// </summary>
        public required string RatchetPublicKey { get; init; }

        /// <summary>
        /// Порядковый номер сообщения — защита от replay-атак
        /// </summary>
        public required int Counter { get; init; }
    }
}