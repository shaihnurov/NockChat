using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления сообщениями чата
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// Создаёт новое сообщение и сохраняет его в базе данных
        /// </summary>
        /// <param name="message">Данные создаваемого сообщения</param>
        /// <param name="ct">Токен</param>
        /// <returns>Созданное сообщение с присвоенным идентификатором</returns>
        Task<Message> CreateAsync(Message message, CancellationToken ct = default);

        /// <summary>
        /// Возвращает постраничный список сообщений указанной комнаты, отсортированных по времени отправки
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="page">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Количество сообщений на странице</param>
        /// <param name="ct">Токен</param>
        /// <returns>Список сообщений на запрошенной странице и общее их количество в комнате</returns>
        Task<(List<Message> Messages, int TotalCount)> GetByRoomAsync(int roomId, int page, int pageSize, CancellationToken ct = default);
    }
}