using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NockChat.Models.Rooms;
using NockChat.Models.Sessions;
using NockChat.Services.Common.DataStorage.Sessions;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.UI;
using NockChat.Services.HTTP.Requests.Rooms;
using NockChat.Services.HTTP.SignalR;

namespace NockChat.ViewModels.Dialogs
{
    public partial class ChatOptionsDialogViewModel(ISignalRService signalRService, INavigationService navigationService, IAppUiState appUiState, 
        ILocalSessionService localSessionService, IRoomRequestsService roomRequestsService, RoomSession session) : ViewModelBase, IDialogContext
    {
        [ObservableProperty]
        public partial ObservableCollection<RoomUserModel> Users { get; set; } = [];

        public event EventHandler<object?>? RequestClose;

        public override async Task Initialize()
        {
            var users = await roomRequestsService.GetRoomUsers(session.Token);

            if(users != null)
                foreach (var user in users)
                    Users.Add(user);
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
        private async Task DeleteRoom()
        {
            await LeaveRoom();
            await roomRequestsService.DeleteRoom(session.Token);
        }

        public void Close()
        {
            RequestClose?.Invoke(this, false);
        }
    }
}