using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Models.Messages;
using NockChat.Models.Sessions;
using NockChat.Services.Common.Exceptions;
using NockChat.Services.Common.Factory;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
using NockChat.Services.HTTP.Requests.Messages;
using NockChat.Services.HTTP.SignalR;
using NockChat.ViewModels.Dialogs;
using NockChat.Views.Dialogs;
using Ursa.Controls;

namespace NockChat.ViewModels
{
    /// <summary>
    /// ViewModel страницы чата
    /// </summary>
    public partial class ChatViewModel(ISignalRService signalRService, IMessageRequestsService messageService, IServiceProvider serviceProvider,
        INotificationService notificationService, INavigationService navigationService, IAppUiState appUiState, RoomSession session) : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Список сообщений текущей комнаты
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<MessageModel> Messages { get; set; } = [];

        /// <summary>
        /// Текст набираемого сообщения
        /// </summary>
        [ObservableProperty]
        public partial string MessageText { get; set; } = string.Empty;

        /// <summary>
        /// Указывает, выполняется ли подключение к комнате
        /// </summary>
        [ObservableProperty]
        public partial bool IsConnecting { get; set; }

        /// <summary>
        /// Вызывается при отправке собственного сообщения
        /// </summary>
        public event Action? OwnMessageSent;

        /// <summary>
        /// Название текущей комнаты
        /// </summary>
        public string RoomName => session.RoomName;
        #endregion

        #region Methods
        /// <summary>
        /// Подключается к комнате через SignalR и загружает историю сообщений
        /// </summary>
        public override async Task Initialize()
        {
            try
            {
                appUiState.IsVisibleMenu = false;
                IsConnecting = true;

                var result = await messageService.GetMessagesAsync(session.Token, 1, 100);
                Messages = new ObservableCollection<MessageModel>(result.Items);

                await signalRService.ConnectAsync(session.Token);
                signalRService.OnMessageReceived(msg =>
                {
                    Dispatcher.UIThread.Post(() => Messages.Add(msg));
                });
            }
            catch (ServerException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (NetworkException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (SignalRException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            finally
            {
                IsConnecting = false;
            }
        }

        /// <summary>
        /// Отправляет сообщение через SignalR
        /// Применяет оптимистичное обновление — сообщение добавляется в список до подтверждения сервера
        /// и удаляется обратно в случае ошибки
        /// </summary>
        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText))
                return;

            var text = MessageText;
            MessageText = string.Empty;

            var optimisticMessage = new MessageModel
            {
                Text = text,
                Username = session.Username,
                SentAt = DateTimeOffset.UtcNow,
                IsOwn = true
            };

            Messages.Add(optimisticMessage);
            OwnMessageSent?.Invoke();

            try
            {
                await signalRService.SendMessageAsync(text);
            }
            catch (SignalRException ex)
            {
                Messages.Remove(optimisticMessage);
                MessageText = text;
                notificationService.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Отключается от SignalR и возвращает пользователя к списку комнат
        /// </summary>
        [RelayCommand]
        private async Task LeaveRoom()
        {
            await signalRService.DisconnectAsync();

            await navigationService.RequestNavigation<ChatListViewModel>();
            appUiState.IsVisibleMenu = true;
        }

        /// <summary>
        /// Открывает диалог настроек комнаты
        /// </summary>
        [RelayCommand]
        private async Task Options()
        {
            try
            {
                var optionsVm = serviceProvider.GetRequiredService<IViewModelFactory<ChatOptionsDialogViewModel>>().Create(session);
                await optionsVm.Initialize();
                await OverlayDialog.ShowCustomAsync<ChatOptionsDialogView, ChatOptionsDialogViewModel, bool>(vm: optionsVm);
            }
            catch (Exception)
            {
                notificationService.ShowError("Не удалось открыть настройки");
            }
        }
        #endregion
    }
}