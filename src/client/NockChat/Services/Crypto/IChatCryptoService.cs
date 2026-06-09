using NockChat.Models.Crypto;

namespace NockChat.Services.Crypto
{
    /// <summary>
    /// Сервис криптографических операций E2E шифрования.
    /// Всё шифрование и дешифрование происходит только на клиенте
    /// </summary>
    public interface IChatCryptoService
    {
        /// <summary>
        /// Генерирует ephemeral keypair на Curve25519.
        /// Вызывается один раз при входе в комнату — новый keypair на каждую сессию
        /// </summary>
        /// <returns>Пара ключей: публичный и приватный в формате byte[]</returns>
        (byte[] publicKey, byte[] privateKey) GenerateKeyPair();

        /// <summary>
        /// Вычисляет общий секрет через ECDH + HKDF.
        /// Оба участника получают одинаковый результат не передавая приватные ключи друг другу.
        /// Сервер видит только публичные ключи и не может восстановить общий секрет
        /// </summary>
        /// <param name="ourPrivateKey">Наш приватный ключ</param>
        /// <param name="theirPublicKey">Публичный ключ собеседника</param>
        /// <param name="roomId">ID комнаты — используется как salt в HKDF, чтобы один и тот же ECDH результат давал разные ключи в разных комнатах</param>
        /// <returns>32-байтовый общий секрет</returns>
        byte[] DeriveSharedSecret(byte[] ourPrivateKey, byte[] theirPublicKey, int roomId);

        /// <summary>
        /// Инициализирует начальное состояние Double Ratchet из общего секрета.
        /// Роль участника определяет направление sending/receiving chain keys —
        /// initiator и responder получают зеркально противоположные ключи,
        /// что обеспечивает корректное шифрование в обе стороны
        /// </summary>
        /// <param name="sharedSecret">Общий секрет полученный через ECDH</param>
        /// <param name="ourPublicKey">Наш ephemeral публичный ключ</param>
        /// <param name="ourPrivateKey">Наш ephemeral приватный ключ</param>
        /// <param name="theirPublicKey">Публичный ключ собеседника</param>
        /// <param name="isInitiator">
        /// Роль в сессии — определяется лексикографическим сравнением публичных ключей.
        /// У обоих участников значение противоположное, поэтому их chain keys автоматически зеркалятся
        /// </param>
        /// <returns>Начальное состояние Double Ratchet для данной пары участников</returns>
        RatchetState InitializeRatchet(byte[] sharedSecret, byte[] ourPublicKey, byte[] ourPrivateKey, byte[] theirPublicKey, bool isInitiator);

        /// <summary>
        /// Шифрует plaintext с продвижением chain ratchet.
        /// Каждый вызов продвигает SendingChainKey и увеличивает SendingCounter —
        /// одинаковый текст каждый раз даёт уникальный шифртекст
        /// </summary>
        /// <param name="state">Текущее состояние ratchet — модифицируется in-place</param>
        /// <param name="plaintext">Открытый текст сообщения</param>
        /// <returns>Зашифрованное сообщение готовое к отправке на сервер</returns>
        EncryptedMessage Encrypt(RatchetState state, string plaintext);

        /// <summary>
        /// Дешифрует входящее сообщение с продвижением chain ratchet.
        /// Если RatchetPublicKey в сообщении отличается от последнего известного —
        /// автоматически выполняет DH ratchet шаг и обновляет весь ключевой материал
        /// </summary>
        /// <param name="state">Текущее состояние ratchet — модифицируется in-place</param>
        /// <param name="message">Зашифрованное сообщение полученное с сервера</param>
        /// <returns>Расшифрованный текст сообщения</returns>
        string Decrypt(RatchetState state, EncryptedMessage message);
    }
}