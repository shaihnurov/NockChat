using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input.Platform;
using NockChat.ViewModels.Dialogs;

namespace NockChat.Views.Dialogs;

public partial class ChatOptionsDialogView : UserControl
{
    public ChatOptionsDialogView(ChatOptionsDialogViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    public ChatOptionsDialogView() => InitializeComponent();

    private async void CopyInviteCode_Click(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (DataContext is not ChatOptionsDialogViewModel vm || string.IsNullOrEmpty(vm.InviteCode?.InviteCode))
            return;

        await (TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(vm.InviteCode.InviteCode) ?? Task.CompletedTask);
        vm.Notification("Код доступа скопирован в буфер обмена", NotificationType.Success);
    }
}