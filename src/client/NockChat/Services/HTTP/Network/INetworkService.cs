using System;

namespace NockChat.Services.HTTP.Network
{
    /// <summary>
    /// Интерфейс мониторинга доступности сети
    /// Является единственным источником истины о состоянии подключения в приложении
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Текущее состояние подключения к интернету
        /// Обновляется периодически через HTTP-ping
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Вызывается при изменении состояния подключения
        /// <c>true</c> — соединение появилось, <c>false</c> — пропало
        /// </summary>
        event EventHandler<bool> ConnectivityChanged;
    }
}