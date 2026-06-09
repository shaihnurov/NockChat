using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Crypto;
using NockChat.Models.Messages;
using NockChat.Models.Rooms;

namespace NockChat.Services.HTTP.SignalR
{
    /// <summary>
    /// Интерфейс для работы с SignalR-подключением к чату
    /// </summary>
    public interface ISignalRService
    {
        /// <summary>
        /// Указывает, установлено ли активное подключение к хабу
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Устанавливает подключение к SignalR-хабу
        /// </summary>
        Task ConnectAsync(string token, CancellationToken ct = default);

        /// <summary>
        /// Разрывает подключение к SignalR-хабу и освобождает ресурсы
        /// </summary>
        Task DisconnectAsync(CancellationToken ct = default);

        /// <summary>
        /// Отправляет зашифрованное сообщение в текущую комнату
        /// </summary>
        Task SendMessageAsync(EncryptedMessage message, CancellationToken ct = default);

        /// <summary>
        /// Публикует наш ephemeral публичный ключ в комнату.
        /// Вызывается сразу после подключения к хабу.
        /// В ответ сервер пришлёт ReceiveRoomKeys с ключами остальных участников.
        /// </summary>
        Task PublishKeyAsync(string ephemeralPublicKey, CancellationToken ct = default);

        /// <summary>
        /// Подписывается на получение входящих зашифрованных сообщений
        /// </summary>
        void OnMessageReceived(Action<MessageModel> handler);

        /// <summary>
        /// Подписывается на получение ключей участников уже находящихся в комнате.
        /// Вызывается один раз сервером в ответ на PublishKey.
        /// </summary>
        void OnReceiveRoomKeys(Action<IReadOnlyList<RoomKeyModel>> handler);

        /// <summary>
        /// Подписывается на событие публикации ключа новым участником.
        /// Вызывается когда кто-то новый входит в комнату после нас.
        /// </summary>
        void OnParticipantKeyPublished(Action<RoomKeyModel> handler);

        /// <summary>
        /// Подписывается на событие входа нового пользователя в комнату
        /// </summary>
        void OnUserJoined(Action<string> handler);

        /// <summary>
        /// Подписывается на событие выхода пользователя из комнаты
        /// </summary>
        void OnUserLeft(Action<string> handler);
    }
}