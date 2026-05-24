using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Sessions;

namespace NockChat.Services.Common.DataStorage.Sessions
{
    /// <summary>
    /// Интерфейс для локального хранения сессий комнат
    /// </summary>
    public interface ILocalSessionService
    {
        /// <summary>
        /// Сохраняет сессию, заменяя существующую для той же комнаты
        /// </summary>
        Task SaveAsync(RoomSession session, CancellationToken ct = default);

        /// <summary>
        /// Загружает все сохранённые сессии
        /// </summary>
        Task<List<RoomSession>> LoadAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Удаляет сессию по токену
        /// </summary>
        Task RemoveAsync(string token, CancellationToken ct = default);
    }
}