using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using NockChat.Services.Attributes;
using NockChat.Services.Common.Notifications;
using NockChat.Services.HTTP;

namespace NockChat.ViewModels
{
    [View("Главная")]
    public partial class HomeViewModel(ILogger<HomeViewModel> logger, IHttpService httpService, INotificationService notificationService) : ViewModelBase
    {
        [RelayCommand]
        private async Task GeneratedChatRoom()
        {
            var response = await httpService.GetAsync<Guid>("/chatroom/id");

            if (response.Success && response.Data != null)
            {
                notificationService.ShowMessage(response.Data.ToString());
            }
        }
    }
}