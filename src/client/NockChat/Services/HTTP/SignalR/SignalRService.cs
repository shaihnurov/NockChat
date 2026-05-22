using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NockChat.Models.Messages;

namespace NockChat.Services.HTTP.SignalR
{
    public class SignalRService(ILogger<SignalRService> logger, IConfiguration configuration) : ISignalRService, IAsyncDisposable
    {
        private HubConnection? _connection;
        private readonly string _hubUrl = configuration["HttpService:HubUrl"]!;

        private string? _activeToken;

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public async Task ConnectAsync(string token, CancellationToken ct = default)
        {
            _activeToken = token;

            if (_connection != null)
                await DisconnectAsync(ct);

            _connection = new HubConnectionBuilder().WithUrl($"{_hubUrl}?access_token={token}").WithAutomaticReconnect().Build();

            try
            {
                await _connection.StartAsync(ct);
                logger.LogInformation("Connected to SignalR hub.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to connect to SignalR hub.");
                throw;
            }
        }

        public async Task DisconnectAsync(CancellationToken ct = default)
        {
            if (_connection == null)
                return;

            try
            {
                await _connection.StopAsync(ct);
                await _connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to disconnect from SignalR hub.");
            }
            finally
            {
                _connection = null;
            }
        }

        public async Task SendMessageAsync(string text, CancellationToken ct = default)
        {
            if (_connection == null || !IsConnected)
                throw new InvalidOperationException("No connection to SignalR hub.");

            if (string.IsNullOrEmpty(_activeToken))
                throw new InvalidOperationException("Token is missing. Call ConnectAsync first.");

            await _connection.InvokeAsync("SendMessage", text, cancellationToken: ct);
        }

        public void OnMessageReceived(Action<MessageModel> handler)
            => _connection?.On("ReceiveMessage", handler);

        public void OnUserJoined(Action<string> handler)
            => _connection?.On("UserJoined", handler);

        public void OnUserLeft(Action<string> handler)
            => _connection?.On("UserLeft", handler);

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
        }
    }
}
