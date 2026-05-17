using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NockChat.Services.Common.DataStorage.Settings;
using NockChat.Services.Common.DependencyInjection;
using NockChat.Services.Common.Extensions;
using NockChat.Services.Common.Extensions.Logger;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
using NockChat.ViewModels;
using NockChat.Views;
using Serilog;
using Serilog.Events;

namespace NockChat;

public partial class App : Application
{
    private IHost? _host;
    private ILogger<App>? _logger;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var appEnvironment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT") ?? "Production";

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{appEnvironment}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((ctx, services) =>
            {
                services.AddCommonServices(ctx.Configuration);
                services.AddTransientServices();
                services.AddSingletonServices();
            })
            .Build();

#if DEBUG
        string logsPath = Path.Combine(AppPaths.LogFolder, "debug-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .Enrich.With<ShortSourceContextEnricher>()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                logsPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
#else
            string logsPath = Path.Combine(AppPaths.LogFolder, "log-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)

                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.With<ExceptionEnricher>()
                .WriteTo.File(
                    logsPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} " +
                    "{ErrorType} {ErrorId} {ErrorMessage}{NewLine}")
                .CreateLogger();
#endif

        _host.Services.GetRequiredService<ILoggerFactory>().AddSerilog();

        _logger = _host.Services.GetRequiredService<ILogger<App>>();

        await _host.StartAsync();

        var mainWindowVm = _host.Services.GetRequiredService<MainViewModel>();
        var notificationService = _host.Services.GetRequiredService<INotificationService>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow(notificationService)
            {
                DataContext = mainWindowVm
            };

            desktop.MainWindow = mainWindow;

            desktop.ShutdownRequested += async (s, e) =>
            {
                e.Cancel = true;
                await ShutdownAsync(desktop);
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activityLifetime)
        {
            activityLifetime.MainViewFactory = () => new MainView(notificationService)
            {
                DataContext = mainWindowVm
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView(notificationService)
            {
                DataContext = mainWindowVm
            };
        }

        await StartApplication();

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Завершает все активные процессы и закрывает приложение
    /// </summary>
    /// <param name="desktop"></param>
    private async Task ShutdownAsync(IClassicDesktopStyleApplicationLifetime desktop)
    {
        if (_host is null)
            return;

        try
        {

        }
        catch (Exception ex)
        {
            _logger!.LogError(ex, "Ошибка при закрытии приложения");
        }
        finally
        {
            await _host.StopAsync();
            desktop.Shutdown();
            _host.Dispose();
        }
    }

    private async Task StartApplication()
    {
        var settingsService = _host!.Services.GetRequiredService<ISettingsService>();
        var appUiState = _host!.Services.GetRequiredService<IAppUiState>();
        var navigation = _host.Services.GetRequiredService<INavigationService>();

        await settingsService.LoadAsync();
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var themeKey = settingsService.Settings.Theme;
            RequestedThemeVariant = themeKey switch
            {
                "Dark" => ThemeVariant.Dark,
                "Light" => ThemeVariant.Light,
                _ => ThemeVariant.Default
            };
        });

        await navigation.RequestNavigation<HomeViewModel>();
        appUiState.IsActiveToggleMenu = true;
    }
}