using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Models.Sessions;
using NockChat.Services.Attributes;
using NockChat.Services.Common.DataStorage.Sessions;
using NockChat.Services.Common.Factory;
using NockChat.Services.Common.Navigations;

namespace NockChat.ViewModels
{
    [View("Комнаты")]
    public partial class ChatListViewModel(ILocalSessionService sessionService, INavigationService navigationService, IServiceProvider serviceProvider) : ViewModelBase
    {
        [ObservableProperty]
        public partial ObservableCollection<RoomSession> Rooms { get; set; } = [];

        [ObservableProperty]
        public partial RoomSession? SelectedRoom { get; set; }

        public override async Task Initialize()
        {
            await LoadRoomsAsync();
        }

        public async Task LoadRoomsAsync(CancellationToken ct = default)
        {
            var sessions = await sessionService.LoadAllAsync(ct);
            Rooms = new ObservableCollection<RoomSession>(sessions);
        }

        [RelayCommand]
        private async Task OpenRoom(RoomSession session)
        {
            var chatViewModel = serviceProvider.GetRequiredService<IViewModelFactory<ChatViewModel>>().Create(session);
            await navigationService.NavigateTo(chatViewModel);
        }
    }
}