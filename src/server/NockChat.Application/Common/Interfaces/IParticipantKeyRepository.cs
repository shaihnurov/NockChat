using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с ephemeral публичными ключами участников
    /// </summary>
    public interface IParticipantKeyRepository
    {
        /// <summary>
        /// Создаёт ключ если его нет, обновляет если есть
        /// Вызывается при каждом входе участника в комнату
        /// </summary>
        Task UpsertAsync(ParticipantKey key, CancellationToken ct = default);

        /// <summary>
        /// Возвращает публичные ключи всех участников комнаты, кроме указанного пользователя
        /// Вызывается при входе в комнату для установки крипто-сессий с уже присутствующими
        /// </summary>
        Task<IReadOnlyList<ParticipantKey>> GetRoomKeysAsync(int roomId, int excludeChatUserId, CancellationToken ct = default);

        /// <summary>
        /// Удаляет ключ участника. Вызывается при отключении от SignalR
        /// </summary>
        Task DeleteAsync(int chatUserId, int roomId, CancellationToken ct = default);
    }
}