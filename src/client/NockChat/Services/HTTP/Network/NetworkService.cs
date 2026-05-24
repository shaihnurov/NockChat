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
    /// Реализация периодического мониторинга доступности сети через HTTP-ping
    /// </summary>
    public sealed class NetworkService : INetworkService, IDisposable
    {
        #region Properties
        private readonly ILogger<NetworkService> _logger;
        private readonly string _pingEndpoint;
        private readonly HttpClient _pingClient;
        private readonly Timer _timer;

        private volatile bool _isOnline;
        private bool _disposed;

        /// <summary>
        /// Интервал между проверками доступности сети
        /// </summary>
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Таймаут одного HTTP-ping запроса
        /// </summary>
        private static readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(5);

        /// <inheritdoc/>
        public bool IsOnline => _isOnline;

        /// <inheritdoc/>
        public event EventHandler<bool>? ConnectivityChanged;
        #endregion

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
        /// Колбэк таймера — сначала проверяет наличие сетевого интерфейса,
        /// затем выполняет реальный HTTP-ping до сервера
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

        /// <summary>
        /// Выполняет HTTP-ping и определяет доступность сервера
        /// </summary>
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
        /// Обновляет состояние подключения и вызывает <see cref="ConnectivityChanged"/> только при реальном изменении
        /// <c>volatile</c> обеспечивает видимость значения между потоками без использования lock
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

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed) 
                return;
            _disposed = true;

            _timer.Dispose();
            _pingClient.Dispose();
        }
    }
}