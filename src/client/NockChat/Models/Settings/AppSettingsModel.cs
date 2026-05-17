namespace NockChat.Models.Settings
{
    /// <summary>
    /// Модель настроек приложения
    /// </summary>
    public class AppSettingsModel
    {
        /// <summary>
        /// Тема приложения
        /// </summary>
        public string Theme { get; set; } = "Default";

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; } = "Guest";

        /// <summary>
        /// Почта пользователя
        /// </summary>
        public string Email { get; set; } = "absent";
    }
}