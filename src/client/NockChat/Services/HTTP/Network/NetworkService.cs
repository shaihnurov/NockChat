using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NockChat.Services.HTTP.Options;

namespace NockChat.Services.HTTP.Network
{
    /// <summary>
    /// Реализация мониторинга сети
    /// </summary>
    public sealed class NetworkService : INetworkService, IDisposable
    {
        private readonly ILogger<NetworkService> _logger;
        private readonly string _pingEndpoint;
        private readonly HttpClient _pingClient;
        private readonly Timer _timer;

        private volatile bool _isOnline;
        private bool _disposed;

        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(5);

        public bool IsOnline => _isOnline;

        public event EventHandler<bool>? ConnectivityChanged;

        public NetworkService(ILogger<NetworkService> logger, IOptions<HttpServiceOptions> options)
        {
            _logger = logger;

            _pingEndpoint = "https://google.com";

            _pingClient = new HttpClient
            {
                Timeout = PingTimeout,
                DefaultRequestVersion = HttpVersion.Version11
            };

            _isOnline = NetworkInterface.GetIsNetworkAvailable();

            _timer = new Timer(callback: OnTimerTick, state: null, dueTime: TimeSpan.Zero, period: PollInterval);
        }

        /// <summary>
        /// Колбэк таймера, выполняет реальную проверку доступности сервера
        /// </summary>
        private async void OnTimerTick(object? _)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                SetOnlineState(false);
                return;
            }

            await CheckServerReachableAsync();
        }

        private async Task CheckServerReachableAsync()
        {
            bool reachable;

            try
            {
                using var cts = new CancellationTokenSource(PingTimeout);
                using var response = await _pingClient.GetAsync(_pingEndpoint, HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(false);

                reachable = true;
            }
            catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or TaskCanceledException)
            {
                reachable = false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Неожиданная ошибка при проверке доступности сервера");
                reachable = false;
            }

            SetOnlineState(reachable);
        }

        /// <summary>
        /// Обновляет состояние и вызывает событие только при реальном изменении.
        /// volatile bool обеспечивает видимость между потоками без lock.
        /// </summary>
        private void SetOnlineState(bool isOnline)
        {
            bool previous = _isOnline;
            _isOnline = isOnline;

            if (isOnline == previous)
                return;

            _logger.LogInformation("Состояние сети изменилось: {Status}", isOnline ? "онлайн" : "оффлайн");
            ConnectivityChanged?.Invoke(this, isOnline);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _timer.Dispose();
            _pingClient.Dispose();
        }
    }
}