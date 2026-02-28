using CommunityToolkit.Mvvm.ComponentModel;

namespace NockChat.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
