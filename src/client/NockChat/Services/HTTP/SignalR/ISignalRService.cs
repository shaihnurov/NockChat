using System;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Messages;

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
        /// <param name="token">Токен сессии комнаты для аутентификации</param>
        Task ConnectAsync(string token, CancellationToken ct = default);

        /// <summary>
        /// Разрывает подключение к SignalR-хабу и освобождает ресурсы
        /// </summary>
        Task DisconnectAsync(CancellationToken ct = default);

        /// <summary>
        /// Отправляет текстовое сообщение в текущую комнату
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        Task SendMessageAsync(string text, CancellationToken ct = default);

        /// <summary>
        /// Подписывается на получение входящих сообщений от сервера
        /// </summary>
        /// <param name="handler">Обработчик входящего сообщения</param>
        void OnMessageReceived(Action<MessageModel> handler);

        /// <summary>
        /// Подписывается на событие входа нового пользователя в комнату
        /// </summary>
        /// <param name="handler">Обработчик, принимающий имя пользователя</param>
        void OnUserJoined(Action<string> handler);

        /// <summary>
        /// Подписывается на событие выхода пользователя из комнаты
        /// </summary>
        /// <param name="handler">Обработчик, принимающий имя пользователя</param>
        void OnUserLeft(Action<string> handler);
    }
}