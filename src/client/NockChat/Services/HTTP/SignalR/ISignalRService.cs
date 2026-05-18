using System;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Messages;

namespace NockChat.Services.HTTP.SignalR
{
    public interface ISignalRService
    {
        bool IsConnected { get; }

        Task ConnectAsync(string token, CancellationToken ct = default);
        Task DisconnectAsync(CancellationToken ct = default);
        Task SendMessageAsync(string text, CancellationToken ct = default);

        void OnMessageReceived(Action<MessageModel> handler);
        void OnUserJoined(Action<string> handler);
        void OnUserLeft(Action<string> handler);
    }
}