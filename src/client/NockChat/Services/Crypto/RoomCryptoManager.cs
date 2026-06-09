using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using NockChat.Models.Crypto;

namespace NockChat.Services.Crypto
{
    /// <summary>
    /// Управляет криптографическими сессиями со всеми участниками одной комнаты.
    /// Создаётся при входе в комнату, уничтожается при выходе.
    /// Все ключи хранятся только в памяти — при перезапуске приложения история нечитаема (пока)
    /// </summary>
    public sealed class RoomCryptoManager : IDisposable
    {
        private readonly IChatCryptoService _crypto;
        private readonly int _roomId;

        private readonly ConcurrentDictionary<int, RatchetState> _sessions = new();

        /// <summary>
        /// Наш ephemeral публичный ключ для текущей сессии — передаётся другим участникам
        /// </summary>
        public byte[] OurPublicKey { get; }

        /// <summary>
        /// Приватный ключ никогда не покидает этот объект
        /// </summary>
        private readonly byte[] _ourPrivateKey;

        /// <summary>
        /// Создаёт менеджер и генерирует новый ephemeral keypair для комнаты
        /// </summary>
        public RoomCryptoManager(IChatCryptoService crypto, int roomId)
        {
            _crypto = crypto;
            _roomId = roomId;
            (OurPublicKey, _ourPrivateKey) = crypto.GenerateKeyPair();
        }

        /// <summary>
        /// Устанавливает крипто-сессию с участником комнаты.
        /// Вызывается когда получаем публичный ключ нового или существующего участника.
        /// При повторном вызове с тем же chatUserId сессия обновляется — участник перезашёл и сгенерировал новый keypair
        /// </summary>
        /// <param name="chatUserId">Идентификатор участника</param>
        /// <param name="theirPublicKey">Ephemeral публичный ключ участника</param>
        public void AddPeer(int chatUserId, byte[] theirPublicKey)
        {
            var sharedSecret = _crypto.DeriveSharedSecret(_ourPrivateKey, theirPublicKey, _roomId);
            var weAreInitiator = IsInitiator(OurPublicKey, theirPublicKey);
            var state = _crypto.InitializeRatchet(sharedSecret, OurPublicKey, _ourPrivateKey, theirPublicKey, weAreInitiator);

            _sessions[chatUserId] = state;
        }

        /// <summary>
        /// Шифрует сообщение для конкретного участника с продвижением его ratchet состояния
        /// </summary>
        /// <param name="chatUserId">Идентификатор получателя</param>
        /// <param name="plaintext">Открытый текст сообщения</param>
        /// <exception cref="InvalidOperationException">Крипто-сессия с участником не установлена</exception>
        public EncryptedMessage EncryptFor(int chatUserId, string plaintext)
        {
            if (!_sessions.TryGetValue(chatUserId, out var state))
                throw new InvalidOperationException($"Нет крипто-сессии с участником {chatUserId}");

            return _crypto.Encrypt(state, plaintext);
        }

        /// <summary>
        /// Дешифрует входящее сообщение от участника с продвижением его ratchet состояния
        /// </summary>
        /// <param name="chatUserId">Идентификатор отправителя</param>
        /// <param name="message">Зашифрованное сообщение</param>
        /// <exception cref="InvalidOperationException">Крипто-сессия с участником не установлена</exception>
        public string DecryptFrom(int chatUserId, EncryptedMessage message)
        {
            if (!_sessions.TryGetValue(chatUserId, out var state))
                throw new InvalidOperationException($"Нет крипто-сессии с участником {chatUserId}");

            return _crypto.Decrypt(state, message);
        }

        /// <summary>
        /// Шифрует сообщение для всех участников с активными сессиями.
        /// Каждый участник получает независимо зашифрованную копию сообщения
        /// </summary>
        /// <param name="plaintext">Открытый текст сообщения</param>
        public IEnumerable<KeyValuePair<int, EncryptedMessage>> EncryptForAll(string plaintext)
        {
            foreach (var (peerId, _) in _sessions)
                yield return new KeyValuePair<int, EncryptedMessage>(peerId, EncryptFor(peerId, plaintext));
        }

        /// <summary>
        /// Возвращает true если крипто-сессия с участником установлена
        /// </summary>
        public bool HasSession(int chatUserId) => _sessions.ContainsKey(chatUserId);

        /// <summary>
        /// Определяет роль участника в сессии через лексикографическое сравнение публичных ключей.
        /// У обоих участников результат противоположный — это гарантирует зеркальность их chain keys
        /// </summary>
        private static bool IsInitiator(byte[] ourKey, byte[] theirKey)
        {
            for (int i = 0; i < Math.Min(ourKey.Length, theirKey.Length); i++)
            {
                if (ourKey[i] != theirKey[i])
                    return ourKey[i] > theirKey[i];
            }
            return ourKey.Length > theirKey.Length;
        }

        /// <summary>
        /// Зачищает весь ключевой материал из памяти.
        /// Вызывается при выходе из комнаты — после этого история сообщений нечитаема (Forward Secrecy)
        /// </summary>
        public void Dispose()
        {
            foreach (var state in _sessions.Values)
            {
                CryptographicOperations.ZeroMemory(state.RootKey);
                CryptographicOperations.ZeroMemory(state.SendingChainKey);
                CryptographicOperations.ZeroMemory(state.ReceivingChainKey);
                CryptographicOperations.ZeroMemory(state.OurRatchetPrivateKey);
            }
            _sessions.Clear();
            CryptographicOperations.ZeroMemory(_ourPrivateKey);
        }
    }
}