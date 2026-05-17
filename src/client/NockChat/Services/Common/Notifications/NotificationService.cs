using System;
using Avalonia.Controls.Notifications;
using Ursa.Controls;

namespace NockChat.Services.Common.Notifications
{
    /// <summary>
    /// Сервис для генерации пользовательских уведомлений
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <inheritdoc/>
        public WindowToastManager? ToastManager { get; set; }

        /// <inheritdoc/>
        public void ShowError(string message) => ShowError(message, ex: null, action: null);
        /// <inheritdoc/>
        public void ShowError(string message, Action action) => ShowError(message, ex: null, action);
        /// <inheritdoc/>
        public void ShowError(string message, Exception ex) => ShowError(message, ex, action: null);

        /// <inheritdoc/>
        public void ShowError(string message, Exception? ex, Action? action)
        {
            if (ex is not null)
                message += Environment.NewLine + Environment.NewLine + ex.ToString();

            ShowMessage(message, title: null, action, delay: TimeSpan.FromSeconds(8), NotificationType.Error);
        }

        /// <inheritdoc/>
		public void ShowMessage(string message) => ShowMessage(message, title: null, action: null, delay: TimeSpan.FromSeconds(5), messageType: NotificationType.Information);

        /// <inheritdoc/>
        public void ShowMessage(string message, string? title) => ShowMessage(message, title, action: null, delay: TimeSpan.FromSeconds(5), messageType: NotificationType.Information);

        /// <inheritdoc/>
        public void ShowMessage(string message, NotificationType messageType = NotificationType.Information) => ShowMessage(message, title: null, action: null, delay: TimeSpan.FromSeconds(5), messageType);

        /// <inheritdoc/>
        public void ShowMessage(string message, TimeSpan delay, NotificationType messageType = NotificationType.Information) => ShowMessage(message, title: null, action: null, delay: delay, messageType);

        /// <inheritdoc/>
        public void ShowMessage(string message, string? title, NotificationType messageType = NotificationType.Information) => ShowMessage(message, title, action: null, delay: TimeSpan.FromSeconds(5), messageType);

        /// <inheritdoc/>
        public void ShowMessage(string message, Action? action, NotificationType messageType = NotificationType.Information) => ShowMessage(message, title: null, action, delay: TimeSpan.FromSeconds(5), messageType);

        /// <inheritdoc/>
        public void ShowMessage(string message, string? title, Action? action, NotificationType messageType = NotificationType.Information) => ShowMessage(message, title, action, delay: TimeSpan.FromSeconds(5), messageType);

        /// <inheritdoc/>
        public void ShowMessage(string message, Action? action, TimeSpan delay, NotificationType messageType = NotificationType.Information) => ShowMessage(message, title: null, action, delay, messageType);

        /// <inheritdoc/>
        public void ShowMessage(string message, string? title, TimeSpan delay, NotificationType messageType = NotificationType.Information) => ShowMessage(message, title, action: null, delay, messageType);

        /// <inheritdoc/>
        public void ShowMessage(string message, string? title, Action? action, TimeSpan delay, NotificationType messageType)
        {
            ToastManager?.Show(new Toast(message),
                type: messageType,
                expiration: delay,
                showIcon: true,
                showClose: false,
                onClick: action);
        }
    }
}