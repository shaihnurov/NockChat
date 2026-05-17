using System;
using Avalonia;

namespace NockChat.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
        if (string.IsNullOrEmpty(environment))
        {
#if DEBUG
            Environment.SetEnvironmentVariable("APP_ENVIRONMENT", "Development");
#else
                Environment.SetEnvironmentVariable("APP_ENVIRONMENT", "Production");
#endif
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
