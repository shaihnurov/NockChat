namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Сервис для криптографических операций E2E шифрования
    /// Используется только на сервере для валидации формата ключей
    /// Само шифрование/дешифрование происходит исключительно на клиенте
    /// </summary>
    public interface IChatCryptoService
    {
        /// <summary>
        /// Проверяет что строка является валидным Curve25519 публичным ключом в Base64
        /// Сервер не генерирует и не хранит приватные ключи
        /// </summary>
        bool IsValidPublicKey(string base64Key);
    }
}