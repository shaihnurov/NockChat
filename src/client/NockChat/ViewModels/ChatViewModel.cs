using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NockChat.Models.Messages;
using NockChat.Models.Sessions;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
using NockChat.Services.HTTP.Requests.Messages;
using NockChat.Services.HTTP.SignalR;

namespace NockChat.ViewModels
{
    public partial class ChatViewModel(ISignalRService signalRService, IMessageRequestsService messageService, 
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

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            try
            {
                appUiState.IsActiveToggleMenu = false;
                IsConnecting = true;

                // Загружаем историю
                var history = await messageService.GetMessagesAsync(session.Token, ct: ct);
                if (history != null)
                    Messages = new ObservableCollection<MessageModel>(history);

                // Подключаемся к SignalR
                await signalRService.ConnectAsync(session.Token, ct);
                signalRService.OnMessageReceived(msg => Messages.Add(msg));

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
            if (string.IsNullOrWhiteSpace(MessageText)) return;

            await signalRService.SendMessageAsync(MessageText);
            MessageText = string.Empty;
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
    }
}