using NockChat.Domain.Entities;

namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления пользователями чата
    /// </summary>
    public interface IChatUserRepository
    {
        /// <summary>
        /// Проверяет, существует ли пользователь с указанным именем в заданной комнате
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="username">Имя пользователя</param>
        /// <param name="ct">Токен</param>
        /// <returns><c>true</c>, если пользователь найден; иначе <c>false</c></returns>
        Task<bool> ExistsAsync(int roomId, string username, CancellationToken ct = default);

        /// <summary>
        /// Получает список всех пользователей, находящихся в указанной комнате
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="ct">Токен</param>
        /// <returns>Список пользователей в комнате</returns>
        Task<IReadOnlyList<ChatUser>> GetRoomUsersAsync(int roomId, CancellationToken ct = default);

        /// <summary>
        /// Создаёт нового пользователя чата и сохраняет его в базе данных
        /// </summary>
        /// <param name="chatUser">Данные создаваемого пользователя</param>
        /// <param name="ct">Токен</param>
        /// <returns>Созданный пользователь с присвоенным идентификатором</returns>
        Task<ChatUser> CreateAsync(ChatUser chatUser, CancellationToken ct = default);

        /// <summary>
        /// Возвращает пользователя чата по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="ct">Токен</param>
        /// <returns>Найденный пользователь или <c>null</c>, если не существует</returns>
        Task<ChatUser?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}