using System;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;

namespace NockChat.Android;

[Application]
public class AndroidApp(IntPtr javaReference, JniHandleOwnership transfer) : AvaloniaAndroidApplication<App>(javaReference, transfer)
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}