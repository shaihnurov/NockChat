using Avalonia.Controls;
using NockChat.Services.Common.Notifications;

namespace NockChat.Views;

public partial class MainView : UserControl
{
    private readonly INotificationService? _notificationService;

#if DEBUG
    public MainView() => InitializeComponent();
#endif

    public MainView(INotificationService notificationService)
    {
        _notificationService = notificationService;

        InitializeComponent();

        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel != null && _notificationService != null)
        {
            _notificationService.ToastManager = new Ursa.Controls.WindowToastManager(topLevel)
            {
                MaxItems = 3,
                Margin = new Avalonia.Thickness(0, 50)
            };
        }
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_notificationService?.ToastManager != null)
        {
            _notificationService.ToastManager.Uninstall();
            _notificationService.ToastManager = null;
        }
    }
}