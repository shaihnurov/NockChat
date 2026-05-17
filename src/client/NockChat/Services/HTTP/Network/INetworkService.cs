using System;

namespace NockChat.Services.HTTP.Network
{
    /// <summary>
    /// Сервис мониторинга доступности сети и серверного API.
    /// Используется как единственный источник истины о состоянии подключения.
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Текущее состояние подключения к интернету и серверу.
        /// Обновляется периодически через HTTP-ping.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Событие, вызываемое при изменении состояния подключения.
        /// <c>true</c> — сеть появилась, <c>false</c> — пропала.
        /// </summary>
        event EventHandler<bool> ConnectivityChanged;
    }
}