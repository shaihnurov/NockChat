namespace NockChat.Services.HTTP.Options
{
    /// <summary>
    /// Настройки HTTP-сервиса, загружаемые из конфигурации приложения (appsettings.json)
    /// </summary>
    public sealed class HttpServiceOptions
    {
        /// <summary>
        /// Базовый URL сервера API
        /// </summary>
        public string BaseUrl { get; init; } = string.Empty;

        /// <summary>
        /// Путь к директории хранилища файлов на сервере
        /// Используется в <see cref="IHttpService.GetStreamAsync"/> для построения полного URL файла
        /// </summary>
        public string StoragePath { get; init; } = "/storage/";
    }
}