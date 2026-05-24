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
using NockChat.Services.Common.Exceptions;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
using NockChat.Services.HTTP.Requests.Rooms;
using NockChat.Services.HTTP.SignalR;

namespace NockChat.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel диалога настроек текущей комнаты чата
    /// </summary>
    public partial class ChatOptionsDialogViewModel(ISignalRService signalRService, INavigationService navigationService, IAppUiState appUiState,
        ILocalSessionService localSessionService, IRoomRequestsService roomRequestsService, INotificationService notificationService,
        RoomSession session) : ViewModelBase, IDialogContext
    {
        #region Properties
        /// <summary>
        /// Список участников текущей комнаты
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<RoomUserModel> Users { get; set; } = [];

        /// <summary>
        /// Код приглашения для входа в комнату
        /// </summary>
        [ObservableProperty]
        public partial InviteCodeModel? InviteCode { get; set; }

        /// <summary>
        /// Указывает, отображается ли блок с кодом приглашения
        /// </summary>
        [ObservableProperty]
        public partial bool IsVisibleInviteCode { get; set; } = false;

        /// <inheritdoc/>
        public event EventHandler<object?>? RequestClose;
        #endregion

        #region Methods
        /// <summary>
        /// Загружает список участников комнаты при открытии диалога
        /// </summary>
        public override async Task Initialize()
        {
            try
            {
                var users = await roomRequestsService.GetRoomUsers(session.Token);
                foreach (var user in users)
                    Users.Add(user);
            }
            catch (ServerException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (NetworkException ex)
            {
                notificationService.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Запрашивает код приглашения для текущей комнаты и отображает его
        /// </summary>
        [RelayCommand]
        private async Task GetAccessCodeRoom()
        {
            try
            {
                InviteCode = await roomRequestsService.GetInviteCodeRoom(session.Token);
                IsVisibleInviteCode = true;
            }
            catch (ServerException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (NetworkException ex)
            {
                notificationService.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Отключается от SignalR, удаляет локальную сессию и возвращает пользователя к списку чатов
        /// </summary>
        [RelayCommand]
        private async Task LeaveRoom()
        {
            try
            {
                await signalRService.DisconnectAsync();
                await localSessionService.RemoveAsync(session.Token);
            }
            catch (StorageException)
            {
                notificationService.ShowError("Не удалось удалить локальную сессию");
            }
            finally
            {
                Close();
                await navigationService.RequestNavigation<ChatListViewModel>();
                appUiState.IsVisibleMenu = true;
            }
        }

        /// <summary>
        /// Исключает участника из комнаты
        /// </summary>
        /// <param name="user">Участник, которого необходимо исключить</param>
        [RelayCommand]
        private async Task KickUser(RoomUserModel user)
        {
        }

        /// <summary>
        /// Удаляет комнату на сервере и выходит из неё
        /// </summary>
        [RelayCommand]
        private async Task DeleteRoom()
        {
            try
            {
                await roomRequestsService.DeleteRoom(session.Token);
                notificationService.ShowMessage("Комната удалена", NotificationType.Success);
                await LeaveRoom();
            }
            catch (ServerException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (NetworkException ex)
            {
                notificationService.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Отображает уведомление с заданным текстом и типом
        /// </summary>
        /// <param name="text">Текст уведомления</param>
        /// <param name="type">Тип уведомления</param>
        public void Notification(string text, NotificationType type)
            => notificationService.ShowMessage(text, type);

        /// <inheritdoc/>
        public void Close()
        {
            RequestClose?.Invoke(this, false);
        }
        #endregion
    }
}