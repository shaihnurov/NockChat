using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NockChat.Services.Attributes;
using NockChat.Services.Common.Exceptions;
using NockChat.Services.Common.Notifications;
using NockChat.Services.HTTP.Requests.Rooms;

namespace NockChat.ViewModels
{
    /// <summary>
    /// ViewModel главной страницы
    /// </summary>
    [View("Главная")]
    public partial class HomeViewModel(INotificationService notificationService, IRoomRequestsService roomRequests) : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Название создаваемой комнаты
        /// </summary>
        [ObservableProperty]
        public partial string? RoomName { get; set; }
        partial void OnRoomNameChanged(string? value) => ClearErrors(nameof(RoomName));

        /// <summary>
        /// Имя пользователя в рамках комнаты
        /// </summary>
        [ObservableProperty]
        public partial string? UserName { get; set; }
        partial void OnUserNameChanged(string? value) => ClearErrors(nameof(UserName));

        /// <summary>
        /// Код доступа для входа в существующую комнату
        /// </summary>
        [ObservableProperty]
        public partial string? AccessCode { get; set; }
        partial void OnAccessCodeChanged(string? value) => ClearErrors(nameof(AccessCode));
        #endregion

        #region Validation
        /// <summary>
        /// Валидирует поля для создания комнаты.
        /// Требует заполнения <see cref="RoomName"/> и <see cref="UserName"/>
        /// </summary>
        private bool ValidateForCreation()
        {
            bool isValid = true;

            ClearErrors(nameof(AccessCode));

            if (string.IsNullOrWhiteSpace(RoomName))
            {
                AddError(nameof(RoomName), "Название комнаты обязательно для заполнения");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(UserName))
            {
                AddError(nameof(UserName), "Имя пользователя обязательно для заполнения");
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Валидирует поля для входа в комнату.
        /// Требует заполнения <see cref="AccessCode"/> и <see cref="UserName"/>
        /// </summary>
        private bool ValidateForJoin()
        {
            bool isValid = true;

            ClearErrors(nameof(RoomName));

            if (string.IsNullOrWhiteSpace(AccessCode))
            {
                AddError(nameof(AccessCode), "Код доступа обязателен для заполнения");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(UserName))
            {
                AddError(nameof(UserName), "Имя пользователя обязательно для заполнения");
                isValid = false;
            }

            return isValid;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Создаёт новую комнату с указанным названием и именем пользователя
        /// </summary>
        [RelayCommand]
        private async Task GeneratedChatRoom()
        {
            if (!ValidateForCreation())
            {
                notificationService.ShowMessage("Пожалуйста, заполните обязательные поля", NotificationType.Warning);
                return;
            }

            try
            {
                var session = await roomRequests.CreateRoom(RoomName!, UserName!);
                notificationService.ShowMessage($"Создана комната: {session.RoomName}", NotificationType.Success);
            }
            catch (ServerException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (NetworkException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (StorageException)
            {
                notificationService.ShowError("Комната создана, но не удалось сохранить сессию локально");
            }
        }

        /// <summary>
        /// Выполняет вход в существующую комнату по коду доступа
        /// </summary>
        [RelayCommand]
        private async Task JoinChatRoom()
        {
            if (!ValidateForJoin())
            {
                notificationService.ShowMessage("Пожалуйста, заполните обязательные поля", NotificationType.Warning);
                return;
            }

            try
            {
                var room = await roomRequests.JoinRoom(AccessCode!, UserName!, default);
                notificationService.ShowMessage($"Вы присоединились к комнате: {room.RoomName}", NotificationType.Success);
            }
            catch (ServerException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (NetworkException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (StorageException)
            {
                notificationService.ShowError("Не удалось сохранить сессию локально");
            }
        }
        #endregion
    }
}