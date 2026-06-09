using NockChat.Application.Common.Interfaces;
using Org.BouncyCastle.Crypto.Parameters;

namespace NockChat.Infrastructure.Services
{
    /// <summary>
    /// Реализация <see cref="IChatCryptoService"/>
    /// Сервер только валидирует формат ключей — не шифрует и не дешифрует
    /// </summary>
    public class ChatCryptoService : IChatCryptoService
    {
        private const int Curve25519KeyLength = 32;

        /// <inheritdoc/>
        public bool IsValidPublicKey(string base64Key)
        {
            if (string.IsNullOrWhiteSpace(base64Key))
                return false;

            try
            {
                Span<byte> keyBytes = stackalloc byte[Curve25519KeyLength];

                if (!Convert.TryFromBase64String(base64Key, keyBytes, out int bytesWritten))
                    return false;

                if (bytesWritten != Curve25519KeyLength)
                    return false;

                _ = new X25519PublicKeyParameters(keyBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}