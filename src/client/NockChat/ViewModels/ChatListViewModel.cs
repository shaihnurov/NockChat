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
using NockChat.Services.Common.Exceptions;
using NockChat.Services.Common.Factory;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;

namespace NockChat.ViewModels
{
    /// <summary>
    /// ViewModel страницы списка комнат
    /// </summary>
    [View("Комнаты")]
    public partial class ChatListViewModel(ILocalSessionService sessionService, INavigationService navigationService, INotificationService notificationService,
        IServiceProvider serviceProvider) : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Список сохранённых сессий комнат
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<RoomSession> Rooms { get; set; } = [];

        /// <summary>
        /// Выбранная комната в списке комнат
        /// </summary>
        [ObservableProperty]
        public partial RoomSession? SelectedRoom { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Загружает список комнат при переходе на страницу
        /// </summary>
        public override async Task Initialize()
        {
            await LoadRoomsAsync();
        }

        /// <summary>
        /// Загружает сохранённые сессии из локального хранилища
        /// </summary>
        public async Task LoadRoomsAsync(CancellationToken ct = default)
        {
            try
            {
                var sessions = await sessionService.LoadAllAsync(ct);
                Rooms = new ObservableCollection<RoomSession>(sessions);
            }
            catch (StorageException)
            {
                notificationService.ShowError("Не удалось загрузить список комнат");
            }
        }

        /// <summary>
        /// Создаёт <see cref="ChatViewModel"/> для выбранной сессии и выполняет навигацию в комнату
        /// </summary>
        /// <param name="session">Сессия комнаты, в которую выполняется переход</param>
        [RelayCommand]
        private async Task OpenRoom(RoomSession session)
        {
            var chatViewModel = serviceProvider.GetRequiredService<IViewModelFactory<ChatViewModel>>().Create(session);
            await navigationService.NavigateTo(chatViewModel);
        }
        #endregion
    }
}