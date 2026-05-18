using System.Diagnostics;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.Input;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.UI;

namespace NockChat.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Зависимости
    public IAppUiState AppUiState { get; }
    private readonly INavigationService _navigationService;
    #endregion

    #region Свойства
    /// <summary>
    /// Текущий установленный UserControl
    /// </summary>
    private ViewModelBase? _currentPage;
    public ViewModelBase? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }
    #endregion

    public MainViewModel(IAppUiState appUiState, INavigationService navigationService)
    {
        AppUiState = appUiState;
        _navigationService = navigationService;

        _navigationService.PageChanged += vm => CurrentPage = vm;
    }

    #region Методы
    #region Навигация
    /// <summary>
    /// Команда для переключения на главную страницу
    /// </summary>
    public RelayCommand RedirectionHomeCommand => new(async () => await _navigationService.RequestNavigation<HomeViewModel>());
    /// <summary>
    /// Команда для переключения на страницу настроек
    /// </summary>
    public RelayCommand RedirectionSettingsCommand => new(async () => await _navigationService.RequestNavigation<HomeViewModel>());
    /// <summary>
    /// Команда для переключения на страницу личного кабинета
    /// </summary>
    public RelayCommand RedirectionChatListCommand => new(async () => await _navigationService.RequestNavigation<ChatListViewModel>());
    #endregion

    /// <summary>
    /// Открывает URL в браузере по умолчанию для различных платформ
    /// </summary>
    /// <param name="url">Ссылка для открытия в браузере</param>
    [RelayCommand]
    private static void OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Process.Start("xdg-open", url);

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Process.Start("open", url);
    }
    #endregion
}