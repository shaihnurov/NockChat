namespace NockChat.Services.Common.UI
{
    /// <summary>
    /// Глобальное состояние пользовательского интерфейса приложения.
    /// Используется для управления элементами оболочки
    /// </summary>
    public interface IAppUiState
    {
        /// <summary>
        /// Заголовок текущей открытой страницы
        /// </summary>
        string? TitlePage { get; set; }

        /// <summary>
        /// Управляет видимостью основного меню управления
        /// </summary>
        bool IsVisibleMenu { get; set; }
    }
}