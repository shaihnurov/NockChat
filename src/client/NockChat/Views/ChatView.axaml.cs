using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using NockChat.ViewModels;

namespace NockChat.Views;

public partial class ChatView : UserControl
{
    public ChatView(ChatViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    public ChatView() => InitializeComponent();

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (DataContext is ChatViewModel vm)
            vm.Messages.CollectionChanged += ScrollToBottom;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ChatViewModel vm)
            vm.Messages.CollectionChanged -= ScrollToBottom;

        base.OnDetachedFromVisualTree(e);
    }

    private void ScrollToBottom(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var scroll = this.FindControl<ScrollViewer>("MessagesScroll");

        if (scroll == null)
            return;

        Dispatcher.UIThread.Post(() => scroll.ScrollToEnd(), DispatcherPriority.Loaded);
    }
}