namespace NockChat.Models.Crypto
{
    /// <summary>
    /// Криптографическое состояние сессии с одним участником комнаты
    /// Хранится только в памяти — при перезапуске приложения история нечитаема
    /// </summary>
    public sealed class RatchetState
    {
        /// <summary>
        /// Корневой ключ — обновляется при каждом DH ratchet шаге
        /// </summary>
        public required byte[] RootKey { get; set; }

        /// <summary>
        /// Ключ цепочки отправки — продвигается при каждом исходящем сообщении
        /// </summary>
        public required byte[] SendingChainKey { get; set; }

        /// <summary>
        /// Ключ цепочки получения — продвигается при каждом входящем сообщении
        /// </summary>
        public required byte[] ReceivingChainKey { get; set; }

        /// <summary>
        /// Наш текущий ephemeral публичный ключ для DH ratchet
        /// </summary>
        public required byte[] OurRatchetPublicKey { get; set; }

        /// <summary>
        /// Наш текущий ephemeral приватный ключ для DH ratchet
        /// </summary>
        public required byte[] OurRatchetPrivateKey { get; set; }

        /// <summary>
        /// Последний известный публичный ключ собеседника
        /// </summary>
        public required byte[] TheirRatchetPublicKey { get; set; }

        /// <summary>
        /// Счётчик отправленных сообщений — используется как AAD для защиты от replay атак
        /// </summary>
        public int SendingCounter { get; set; }

        /// <summary>
        /// Счётчик полученных сообщений
        /// </summary>
        public int ReceivingCounter { get; set; }
    }
}