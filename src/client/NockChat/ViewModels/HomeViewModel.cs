using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NockChat.Services.Attributes;
using NockChat.Services.Common.Notifications;
using NockChat.Services.HTTP.Requests.Rooms;

namespace NockChat.ViewModels
{
    [View("Главная")]
    public partial class HomeViewModel(INotificationService notificationService, IRoomRequestsService roomRequests) : ViewModelBase
    {
        [ObservableProperty] 
        public partial string? RoomName { get; set; }
        partial void OnRoomNameChanged(string? value) => ClearErrors(nameof(RoomName));

        [ObservableProperty] 
        public partial string? UserName { get; set; }
        partial void OnUserNameChanged(string? value) => ClearErrors(nameof(UserName));

        [ObservableProperty] 
        public partial string? AccessCode { get; set; }
        partial void OnAccessCodeChanged(string? value) => ClearErrors(nameof(AccessCode));

        /// <summary>
        /// Валидация только для создания комнаты
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
        /// Валидация только для входа в комнату
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

        [RelayCommand]
        private async Task GeneratedChatRoom()
        {
            if (!ValidateForCreation())
            {
                notificationService.ShowMessage("Пожалуйста, заполните обязательные поля", NotificationType.Warning);
                return;
            }

            var room = await roomRequests.CreateRoom(RoomName!, UserName!, default);
            if (room != null)
                notificationService.ShowMessage($"Создана комната: {room.RoomName}", NotificationType.Success);
        }

        [RelayCommand]
        private async Task JoinChatRoom()
        {
            if (!ValidateForJoin())
            {
                notificationService.ShowMessage("Пожалуйста, заполните обязательные поля", NotificationType.Warning);
                return;
            }

            var room = await roomRequests.JoinRoom(AccessCode!, UserName!, default);
            if (room != null)
                notificationService.ShowMessage($"Вы вошли в комнату: {room.RoomName}", NotificationType.Success);
        }
    }
}