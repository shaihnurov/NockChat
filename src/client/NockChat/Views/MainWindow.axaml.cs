using System.Threading.Tasks;
using NockChat.Services.Common.Notifications;
using Ursa.Controls;

namespace NockChat.Views;

public partial class MainWindow : UrsaWindow
{
    private readonly INotificationService? _notificationService;

#if DEBUG
    public MainWindow() => InitializeComponent();
#endif

    public MainWindow(INotificationService notificationService)
    {
        InitializeComponent();

        _notificationService = notificationService;
        Opened += OnWindowOpened;
    }

    private void OnWindowOpened(object? sender, System.EventArgs e)
    {
        _notificationService?.ToastManager = new WindowToastManager(this) { MaxItems = 3 };
    }

    protected override async Task<bool> CanClose()
    {
        var result = await OverlayMessageBox.ShowAsync("Вы уверены, что хотите выйти?", "Выход", button: MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }

    protected override void OnClosed(System.EventArgs e)
    {
        base.OnClosed(e);

        if (_notificationService?.ToastManager != null)
        {
            _notificationService.ToastManager.Uninstall();
            _notificationService.ToastManager = null;
        }
    }
}