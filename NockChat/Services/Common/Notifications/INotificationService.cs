using System;
using Avalonia.Controls.Notifications;
using Ursa.Controls;

namespace NockChat.Services.Common.Notifications
{
    /// <summary>
    /// Интерфейс для отображения пользовательских уведомлений
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Хранит ссылку на менеджер уведомлений, который отвечает за отображение уведомлений в окне приложения
        /// </summary>
        WindowToastManager? ToastManager { get; set; }

        /// <summary>
        /// Показывает сообщение об ошибке
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        void ShowError(string message);

        /// <summary>
        /// Показывает сообщение об ошибке c Action
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="action">Переданный Action</param>
        void ShowError(string message, Action action);

        /// <summary>
        /// Показывает сообщение об ошибке c Exception
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="ex">Исключение, связанное с ошибкой (если есть).</param>
        void ShowError(string message, Exception ex);

        /// <summary>
        /// Показывает сообщение об ошибке с привязкой исключения
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="ex">Исключение, связанное с ошибкой (если есть).</param>
        void ShowError(string message, Exception? ex, Action? action);

        /// <summary>
        /// Показывает простое уведомление
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        void ShowMessage(string message);

        /// <summary>
        /// Показывает уведомление с заголовком
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="title">Заголовок уведомления (необязательный).</param>
        void ShowMessage(string message, string? title);

        /// <summary>
        /// Показывает уведомление определённого типа
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, NotificationType messageType = NotificationType.Information);

        /// <summary>
        /// Показывает уведомление определённого типа
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="delay">Время жизни уведомления</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, TimeSpan delay, NotificationType messageType = NotificationType.Information);

        /// <summary>
        /// Показывает уведомление определённого типа
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="title">Заголовок уведомления (необязательный).</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, string? title, NotificationType messageType = NotificationType.Information);

        /// <summary>
        /// Показывает уведомление с Action
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="action">Переданный Action</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, Action? action, NotificationType messageType = NotificationType.Information);

        /// <summary>
        /// Показывает уведомление с Action
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="title">Заголовок уведомления (необязательный).</param>
        /// <param name="action">Переданный Action</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, string? title, Action? action, NotificationType messageType = NotificationType.Information);

        /// <summary>
        /// Показывает уведомление с Action
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="action">Переданный Action</param>
        /// <param name="delay">Время жизни уведомления</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, Action? action, TimeSpan delay, NotificationType messageType = NotificationType.Information);

        /// <summary>
        /// Показывает уведомление с Action
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="title">Заголовок уведомления (необязательный).</param>
        /// <param name="action">Переданный Action</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, string? title, TimeSpan delay, NotificationType messageType = NotificationType.Information);

        /// <summary>
        /// Показывает уведомление с заголовком и типом
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        /// <param name="title">Заголовок уведомления (необязательный).</param>
        /// <param name="action">Переданный Action</param>
        /// <param name="delay">Время жизни уведомления</param>
        /// <param name="messageType">Тип уведомления (информация, ошибка, предупреждение и т.д.).</param>
        void ShowMessage(string message, string? title, Action? action, TimeSpan delay, NotificationType messageType = NotificationType.Information);
    }
}