using System.Threading.Tasks;
using NockChat.Models.Settings;

namespace NockChat.Services.Common.DataStorage.Settings
{
    /// <summary>
    /// Интерфейс для управления настройками приложения
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Текущие настройки приложения
        /// </summary>
        AppSettingsModel Settings { get; }

        /// <summary>
        /// Загружает настройки из хранилища
        /// </summary>
        Task LoadAsync();

        /// <summary>
        /// Сохраняет текущие настройки в хранилище
        /// </summary>
        Task SaveAsync();
    }
}