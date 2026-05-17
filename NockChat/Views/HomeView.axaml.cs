using Avalonia.Controls;
using NockChat.ViewModels;

namespace NockChat.Views;

public partial class HomeView : UserControl
{
    public HomeView(HomeViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    public HomeView() => InitializeComponent();
}