namespace NockChat.Services.Common.Extensions
{
    /// <summary>
    /// Интерфейс, который позволяет реализовать и вызвать метод
    /// для очистки событий
    /// </summary>
    public interface IEventCleaning
    {
        /// <summary>
        /// Метод для отписки от событий
        /// </summary>
        void CleanEvent();
    }
}
