using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NockChat.Models.Crypto;
using NockChat.Models.Messages;
using NockChat.Models.Rooms;
using NockChat.Services.Common.Exceptions;

namespace NockChat.Services.HTTP.SignalR
{
    /// <summary>
    /// Реализация SignalR-клиента для обмена сообщениями в реальном времени
    /// </summary>
    public class SignalRService(ILogger<SignalRService> logger, IConfiguration configuration) : ISignalRService, IAsyncDisposable
    {
        private HubConnection? _connection;
        private readonly string _hubUrl = configuration["HttpService:HubUrl"]!;

        /// <inheritdoc/>
        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        /// <inheritdoc/>
        public async Task ConnectAsync(string token, CancellationToken ct = default)
        {
            if (_connection != null)
                await DisconnectAsync(ct);

            _connection = new HubConnectionBuilder().WithUrl($"{_hubUrl}?access_token={token}").WithAutomaticReconnect().Build();

            try
            {
                await _connection.StartAsync(ct);
                logger.LogInformation("Connected to SignalR hub.");
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to connect to SignalR hub.");
                throw new SignalRException("Не удалось подключиться к чату", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DisconnectAsync(CancellationToken ct = default)
        {
            if (_connection == null)
                return;

            try
            {
                await _connection.StopAsync(ct);
                await _connection.DisposeAsync();
                logger.LogInformation("Disconnected from SignalR hub.");
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

        /// <inheritdoc/>
        public async Task SendMessageAsync(EncryptedMessage message, CancellationToken ct = default)
        {
            if (_connection == null || !IsConnected)
                throw new SignalRException("Нет подключения к чату");

            try
            {
                await _connection.InvokeAsync("SendMessage", message, cancellationToken: ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send message.");
                throw new SignalRException("Не удалось отправить сообщение", ex);
            }
        }

        /// <inheritdoc/>
        public async Task PublishKeyAsync(string ephemeralPublicKey, CancellationToken ct = default)
        {
            if (_connection == null || !IsConnected)
                throw new SignalRException("Нет подключения к чату");

            try
            {
                await _connection.InvokeAsync("PublishKey", ephemeralPublicKey, cancellationToken: ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish key.");
                throw new SignalRException("Не удалось опубликовать ключ шифрования", ex);
            }
        }

        /// <inheritdoc/>
        public void OnMessageReceived(Action<MessageModel> handler)
            => _connection?.On("ReceiveMessage", handler);

        /// <inheritdoc/>
        public void OnReceiveRoomKeys(Action<IReadOnlyList<RoomKeyModel>> handler)
            => _connection?.On("ReceiveRoomKeys", handler);

        /// <inheritdoc/>
        public void OnParticipantKeyPublished(Action<RoomKeyModel> handler)
            => _connection?.On("ParticipantKeyPublished", handler);

        /// <inheritdoc/>
        public void OnUserJoined(Action<string> handler)
            => _connection?.On("UserJoined", handler);

        /// <inheritdoc/>
        public void OnUserLeft(Action<string> handler)
            => _connection?.On("UserLeft", handler);

        /// <summary>
        /// Разрывает подключение и освобождает все ресурсы сервиса
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
            GC.SuppressFinalize(this);
        }
    }
}