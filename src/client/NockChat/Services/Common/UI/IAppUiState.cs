namespace NockChat.Services.Common.UI
{
    /// <summary>
    /// Глобальное состояние пользовательского интерфейса приложения.
    /// Используется для управления элементами оболочки
    /// </summary>
    public interface IAppUiState
    {
        /// <summary>
        /// Указывает, присутствует ли ошибка в UI
        /// </summary>
        bool HasError { get; set; }

        /// <summary>
        /// Указывает, установленно ли подключение VPN
        /// </summary>
        bool HasConnection { get; set; }

        /// <summary>
        /// Сообщение, отображаемое в статусной строке
        /// </summary>
        string? StatusBarMessage { get; set; }

        /// <summary>
        /// Заголовок текущей открытой страницы
        /// </summary>
        string? TitlePage { get; set; }

        /// <summary>
        /// Управляет доступностью кнопки открытия меню
        /// </summary>
        bool IsActiveToggleMenu { get; set; }
    }
}