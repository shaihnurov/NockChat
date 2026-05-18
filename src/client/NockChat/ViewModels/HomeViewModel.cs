using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NockChat.Services.Attributes;
using NockChat.Services.Common.Notifications;
using NockChat.Services.HTTP.Requests;

namespace NockChat.ViewModels
{
    [View("Главная")]
    public partial class HomeViewModel(INotificationService notificationService, IRoomRequestsService roomRequests) : ViewModelBase
    {
        [ObservableProperty]
        public partial string? RoomName { get; set; }
        partial void OnRoomNameChanged(string? value)
        {
            ClearErrors(nameof(RoomName));
            if (string.IsNullOrWhiteSpace(value))
                AddError(nameof(RoomName), "Название комнаты обязательно для заполнения");
        }

        [ObservableProperty]
        public partial string? UserName { get; set; }
        partial void OnUserNameChanged(string? value)
        {
            ClearErrors(nameof(UserName));
            if (string.IsNullOrWhiteSpace(value))
                AddError(nameof(UserName), "Имя пользователя обязательно для заполнения");
        }

        private bool ValidateAll()
        {
            OnRoomNameChanged(RoomName);
            OnUserNameChanged(UserName);

            return !HasErrors;
        }


        [RelayCommand]
        private async Task GeneratedChatRoom()
        {
            if (!ValidateAll())
            {
                notificationService.ShowMessage("Пожалуйста, исправьте ошибки в форме", NotificationType.Warning);
                return;
            }

            var room = await roomRequests.CreateRoom(RoomName!, UserName!, default);
            if (room != null)
                notificationService.ShowMessage($"Создана новая чат-комната: {room.RoomName}, {room.JoinedAt}", NotificationType.Success);
        }
    }
}