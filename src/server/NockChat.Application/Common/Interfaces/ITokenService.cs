namespace NockChat.Application.Common.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для генерации JWT-токенов доступа к комнате чата
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Генерирует JWT-токен с клеймами пользователя и комнаты
        /// </summary>
        /// <param name="chatUserId">Идентификатор пользователя чата</param>
        /// <param name="roomId">Идентификатор комнаты</param>
        /// <param name="roomName">Название комнаты</param>
        /// <param name="username">Имя пользователя</param>
        /// <returns>Подписанный JWT-токен в виде строки</returns>
        string GenerateToken(int chatUserId, int roomId, string roomName, string username);
    }
}