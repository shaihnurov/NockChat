using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Models.Messages;
using NockChat.Models.Sessions;
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
    public partial class ChatViewModel(ISignalRService signalRService, IMessageRequestsService messageService, IServiceProvider serviceProvider,
        INotificationService notificationService, INavigationService navigationService, IAppUiState appUiState, RoomSession session) : ViewModelBase
    {
        [ObservableProperty]
        public partial ObservableCollection<MessageModel> Messages { get; set; } = [];

        [ObservableProperty]
        public partial string MessageText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsConnecting { get; set; }

        [ObservableProperty]
        public partial bool IsAccessCodeVisible { get; set; }

        public string RoomName => session.RoomName;
        public string Username => session.Username;

        public override async Task Initialize()
        {
            try
            {
                appUiState.IsActiveToggleMenu = false;
                IsConnecting = true;

                // Загружаем историю
                var result = await messageService.GetMessagesAsync(session.Token, 1, 100);
                if (result != null)
                    Messages = new ObservableCollection<MessageModel>(result.Items);

                // Подключаемся к SignalR
                await signalRService.ConnectAsync(session.Token);
                signalRService.OnMessageReceived(msg =>
                {
                    if (msg.Username == session.Username)
                        return;

                    Messages.Add(msg);
                });

                IsConnecting = false;
            }
            catch (Exception)
            {
                notificationService.ShowError("Возникла неожиданная ошибка. Подробности см. в журнале");
            }
        }

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
                SentAt = DateTime.UtcNow,
                IsOwn = true
            };

            Messages.Add(optimisticMessage);

            try
            {
                await signalRService.SendMessageAsync(text);
            }
            catch (Exception)
            {
                Messages.Remove(optimisticMessage);
                MessageText = text;
                notificationService.ShowError("Не удалось отправить сообщение");
            }
        }

        [RelayCommand]
        private void ToggleAccessCode() => IsAccessCodeVisible = !IsAccessCodeVisible;

        [RelayCommand]
        private async Task LeaveRoom()
        {
            await signalRService.DisconnectAsync();

            await navigationService.RequestNavigation<ChatListViewModel>();
            appUiState.IsActiveToggleMenu = true;
        }

        [RelayCommand]
        private async Task Options()
        {
            var optionsVm = serviceProvider.GetRequiredService<IViewModelFactory<ChatOptionsDialogViewModel>>().Create(session);
            await optionsVm.Initialize();
            await OverlayDialog.ShowCustomAsync<ChatOptionsDialogView, ChatOptionsDialogViewModel, bool>(vm: optionsVm);
        }
    }
}