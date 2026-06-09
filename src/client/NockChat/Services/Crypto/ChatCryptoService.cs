using System;
using System.Security.Cryptography;
using System.Text;
using NockChat.Models.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NockChat.Services.Crypto
{
    /// <summary>
    /// Реализация <see cref="IChatCryptoService"/> на основе Curve25519 + AES-256-GCM + Double Ratchet.
    /// Curve25519 используется для ECDH обмена ключами, HKDF-SHA256 для деривации ключевого материала,
    /// AES-256-GCM для аутентифицированного шифрования каждого сообщения
    /// </summary>
    public sealed class ChatCryptoService : IChatCryptoService
    {
        private static readonly SecureRandom SecureRandom = new();

        /// <inheritdoc/>
        public (byte[] publicKey, byte[] privateKey) GenerateKeyPair()
        {
            var generator = new X25519KeyPairGenerator();
            generator.Init(new X25519KeyGenerationParameters(SecureRandom));

            var keyPair = generator.GenerateKeyPair();
            var pub = ((X25519PublicKeyParameters)keyPair.Public).GetEncoded();
            var priv = ((X25519PrivateKeyParameters)keyPair.Private).GetEncoded();
            return (pub, priv);
        }

        /// <inheritdoc/>
        public byte[] DeriveSharedSecret(byte[] ourPrivateKey, byte[] theirPublicKey, int roomId)
        {
            var privParams = new X25519PrivateKeyParameters(ourPrivateKey);
            var pubParams = new X25519PublicKeyParameters(theirPublicKey);

            var agreement = new X25519Agreement();
            agreement.Init(privParams);

            var rawShared = new byte[agreement.AgreementSize];
            agreement.CalculateAgreement(pubParams, rawShared);

            var salt = BitConverter.GetBytes(roomId);
            var info = Encoding.UTF8.GetBytes("NockChat-v1");
            return HKDF.DeriveKey(HashAlgorithmName.SHA256, rawShared, 32, salt, info);
        }

        /// <inheritdoc/>
        public RatchetState InitializeRatchet(byte[] sharedSecret, byte[] ourPublicKey, byte[] ourPrivateKey, byte[] theirPublicKey, bool isInitiator)
        {
            var sendInfo = isInitiator ? Encoding.UTF8.GetBytes("NockChat-send") : Encoding.UTF8.GetBytes("NockChat-recv");
            var recvInfo = isInitiator ? Encoding.UTF8.GetBytes("NockChat-recv") : Encoding.UTF8.GetBytes("NockChat-send");

            var rootKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, 32, null, Encoding.UTF8.GetBytes("NockChat-root"));
            var sendingChainKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, 32, null, sendInfo);
            var receivingChainKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, 32, null, recvInfo);

            return new RatchetState
            {
                RootKey = rootKey,
                SendingChainKey = sendingChainKey,
                ReceivingChainKey = receivingChainKey,
                OurRatchetPublicKey = ourPublicKey,
                OurRatchetPrivateKey = ourPrivateKey,
                TheirRatchetPublicKey = theirPublicKey
            };
        }

        /// <inheritdoc/>
        public EncryptedMessage Encrypt(RatchetState state, string plaintext)
        {
            var (messageKey, nextChainKey) = RatchetStep(state.SendingChainKey);
            state.SendingChainKey = nextChainKey;
            state.SendingCounter++;

            var nonce = RandomNumberGenerator.GetBytes(12);
            var aad = BitConverter.GetBytes(state.SendingCounter);
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            var ciphertext = new byte[plaintextBytes.Length + 16];

            using var aes = new AesGcm(messageKey, tagSizeInBytes: 16);
            aes.Encrypt(nonce: nonce, plaintext: plaintextBytes, ciphertext: ciphertext.AsSpan(0, plaintextBytes.Length),
                tag: ciphertext.AsSpan(plaintextBytes.Length), associatedData: aad);

            CryptographicOperations.ZeroMemory(messageKey);

            return new EncryptedMessage
            {
                Nonce = Convert.ToBase64String(nonce),
                Ciphertext = Convert.ToBase64String(ciphertext),
                RatchetPublicKey = Convert.ToBase64String(state.OurRatchetPublicKey),
                Counter = state.SendingCounter
            };
        }

        /// <inheritdoc/>
        public string Decrypt(RatchetState state, EncryptedMessage message)
        {
            var theirCurrentKey = Convert.FromBase64String(message.RatchetPublicKey);

            if (!theirCurrentKey.AsSpan().SequenceEqual(state.TheirRatchetPublicKey))
                AdvanceDhRatchet(state, theirCurrentKey);

            var (messageKey, nextChainKey) = RatchetStep(state.ReceivingChainKey);
            state.ReceivingChainKey = nextChainKey;
            state.ReceivingCounter++;

            var nonce = Convert.FromBase64String(message.Nonce);
            var ciphertext = Convert.FromBase64String(message.Ciphertext);
            var aad = BitConverter.GetBytes(message.Counter);
            var plaintext = new byte[ciphertext.Length - 16];

            using var aes = new AesGcm(messageKey, tagSizeInBytes: 16);
            aes.Decrypt(nonce: nonce, ciphertext: ciphertext.AsSpan(0, plaintext.Length), tag: ciphertext.AsSpan(plaintext.Length),
                plaintext: plaintext, associatedData: aad);

            CryptographicOperations.ZeroMemory(messageKey);
            return Encoding.UTF8.GetString(plaintext);
        }

        /// <summary>
        /// Один шаг симметричного chain ratchet на основе HMAC-SHA256.
        /// Из одного chain key получаем два независимых ключа:
        /// message key для шифрования и новый chain key для следующего шага.
        /// Однонаправленность HMAC гарантирует Forward Secrecy —
        /// зная message key невозможно восстановить предыдущий chain key
        /// </summary>
        /// <param name="chainKey">Текущий ключ цепочки</param>
        /// <returns>Одноразовый message key и следующий chain key</returns>
        private static (byte[] messageKey, byte[] nextChainKey) RatchetStep(byte[] chainKey)
        {
            using var hmac = new HMACSHA256(chainKey);
            var messageKey = hmac.ComputeHash([0x01]);
            var nextChainKey = hmac.ComputeHash([0x02]);
            return (messageKey, nextChainKey);
        }

        /// <summary>
        /// Шаг асимметричного DH ratchet — полная смена ключевого материала.
        /// Выполняется когда собеседник прислал новый ratchet публичный ключ.
        /// Новый ECDH + HKDF обновляет root key и оба chain key,
        /// обеспечивая Break-in Recovery: компрометация текущих ключей
        /// не раскрывает будущие сообщения после следующего DH шага
        /// </summary>
        /// <param name="state">Состояние ratchet — модифицируется in-place</param>
        /// <param name="theirNewPublicKey">Новый публичный ключ собеседника из входящего сообщения</param>
        private void AdvanceDhRatchet(RatchetState state, byte[] theirNewPublicKey)
        {
            var (newPub, newPriv) = GenerateKeyPair();

            var privParams = new X25519PrivateKeyParameters(state.OurRatchetPrivateKey);
            var pubParams = new X25519PublicKeyParameters(theirNewPublicKey);
            var agreement = new X25519Agreement();
            agreement.Init(privParams);

            var dhBytes = new byte[agreement.AgreementSize];
            agreement.CalculateAgreement(pubParams, dhBytes);

            state.RootKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, dhBytes, 32, state.RootKey, Encoding.UTF8.GetBytes("NockChat-ratchet-root"));
            state.ReceivingChainKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, dhBytes, 32, state.RootKey, Encoding.UTF8.GetBytes("NockChat-ratchet-recv"));
            state.SendingChainKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, dhBytes, 32, state.RootKey, Encoding.UTF8.GetBytes("NockChat-ratchet-send"));

            state.TheirRatchetPublicKey = theirNewPublicKey;
            state.OurRatchetPublicKey = newPub;
            state.OurRatchetPrivateKey = newPriv;
        }
    }
}