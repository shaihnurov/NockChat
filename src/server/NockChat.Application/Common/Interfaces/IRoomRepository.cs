using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления комнатами чата
    /// </summary>
    public interface IRoomRepository
    {
        /// <summary>
        /// Создаёт новую комнату и сохраняет её в базе данных
        /// </summary>
        /// <param name="room">Данные создаваемой комнаты</param>
        /// <param name="ct">Токен</param>
        /// <returns>Созданная комната с присвоенным идентификатором</returns>
        Task<Room> CreateAsync(Room room, CancellationToken ct = default);

        /// <summary>
        /// Возвращает комнату по идентификатору
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="ct">Токен</param>
        /// <returns>Комната или <c>null</c> если не найдена</returns>
        Task<Room?> GetByIdAsync(int roomId, CancellationToken ct = default);

        /// <summary>
        /// Обновляет временный код приглашения комнаты
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="code">Новый код приглашения</param>
        /// <param name="expiresAt">Время истечения кода</param>
        /// <param name="ct">Токен</param>
        Task UpdateInviteCodeAsync(int roomId, string code, DateTime expiresAt, CancellationToken ct = default);

        /// <summary>
        /// Возвращает комнату по действующему коду приглашения
        /// </summary>
        /// <param name="inviteCode">Временный код приглашения</param>
        /// <param name="ct">Токен</param>
        /// <returns>Комната или <c>null</c> если код не найден или истёк</returns>
        Task<Room?> GetByInviteCodeAsync(string inviteCode, CancellationToken ct = default);

        /// <summary>
        /// Удаляет выбранную комнату
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="ct">Токен</param>
        Task DeleteAsync(int roomId, CancellationToken ct = default);
    }
}