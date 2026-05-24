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
        {
            vm.Messages.CollectionChanged += ScrollToBottom;
            vm.OwnMessageSent += ForceScrollToBottom;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ChatViewModel vm)
        {
            vm.Messages.CollectionChanged -= ScrollToBottom;
            vm.OwnMessageSent -= ForceScrollToBottom;
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void ForceScrollToBottom()
    {
        var scroll = this.FindControl<ScrollViewer>("MessagesScroll");
        Dispatcher.UIThread.Post(() => scroll?.ScrollToEnd(), DispatcherPriority.Loaded);
    }

    private void ScrollToBottom(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add)
            return;

        var scroll = this.FindControl<ScrollViewer>("MessagesScroll");
        if (scroll == null)
            return;

        var isNearBottom = scroll.Extent.Height - scroll.Offset.Y - scroll.Viewport.Height < 100;

        if (isNearBottom)
            Dispatcher.UIThread.Post(() => scroll.ScrollToEnd(), DispatcherPriority.Loaded);
    }
}