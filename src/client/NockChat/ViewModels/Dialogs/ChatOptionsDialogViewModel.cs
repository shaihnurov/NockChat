using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NockChat.Models.Rooms;
using NockChat.Models.Sessions;
using NockChat.Services.Common.DataStorage.Sessions;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
using NockChat.Services.HTTP.Requests.Rooms;
using NockChat.Services.HTTP.SignalR;

namespace NockChat.ViewModels.Dialogs
{
    public partial class ChatOptionsDialogViewModel(ISignalRService signalRService, INavigationService navigationService, IAppUiState appUiState,
        ILocalSessionService localSessionService, IRoomRequestsService roomRequestsService, INotificationService notificationService,
        RoomSession session) : ViewModelBase, IDialogContext
    {
        [ObservableProperty]
        public partial ObservableCollection<RoomUserModel> Users { get; set; } = [];

        [ObservableProperty]
        public partial InviteCodeModel? InviteCode { get; set; }

        [ObservableProperty]
        public partial bool IsVisibleInviteCode { get; set; } = false;

        public event EventHandler<object?>? RequestClose;

        public override async Task Initialize()
        {
            var users = await roomRequestsService.GetRoomUsers(session.Token);

            if (users != null)
                foreach (var user in users)
                    Users.Add(user);
        }

        [RelayCommand]
        private async Task GetAccessCodeRoom()
        {
            InviteCode = await roomRequestsService.GetInviteCodeRoom(session.Token);

            if (InviteCode == null || InviteCode.InviteCode == null)
            {
                notificationService.ShowError("Не удалось получить код доступа к комнате");
                return;
            }

            IsVisibleInviteCode = true;
        }

        [RelayCommand]
        private async Task LeaveRoom()
        {
            await signalRService.DisconnectAsync();
            await localSessionService.RemoveAsync(session.Token);

            Close();
            await navigationService.RequestNavigation<ChatListViewModel>();
            appUiState.IsActiveToggleMenu = true;
        }

        [RelayCommand]
        private async Task KickUser(RoomUserModel user)
        {
        }

        [RelayCommand]
        private async Task DeleteRoom()
        {
            await LeaveRoom();
            await roomRequestsService.DeleteRoom(session.Token);
        }

        public void Notification(string text, NotificationType type)
            => notificationService.ShowMessage(text, type);

        public void Close()
        {
            RequestClose?.Invoke(this, false);
        }
    }
}