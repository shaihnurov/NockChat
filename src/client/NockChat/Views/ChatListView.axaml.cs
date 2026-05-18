using Avalonia.Controls;
using NockChat.ViewModels;

namespace NockChat.Views;

public partial class ChatListView : UserControl
{
    public ChatListView(ChatListViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    public ChatListView() => InitializeComponent();
}