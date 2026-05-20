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
        /// Возвращает комнату по её коду доступа
        /// </summary>
        /// <param name="accessCode">Уникальный код доступа к комнате</param>
        /// <param name="ct">Токен</param>
        /// <returns>Найденная комната или <c>null</c>, если не существует</returns>
        Task<Room?> GetByAccessCodeAsync(string accessCode, CancellationToken ct = default);

        /// <summary>
        /// Удаляет выбранную комнату
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="ct">Токен</param>
        Task DeleteAsync(int roomId, CancellationToken ct = default);
    }
}