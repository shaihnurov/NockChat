using Avalonia.Controls;
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
}