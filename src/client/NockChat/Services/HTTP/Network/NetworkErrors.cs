namespace NockChat.Services.HTTP.Network
{
    /// <summary>
    /// Константы сетевых ошибок для единообразной обработки во всём приложении
    /// </summary>
    public static class NetworkErrors
    {
        public const string NoConnection = "Нет подключения к интернету";
        public const string Timeout = "Таймаут запроса к серверу";
        public const string Cancelled = "Запрос отменён";
        public const string ServerUnavailable = "Ошибка подключения к серверу";
        public const string DeserializationError = "Ошибка десериализации ответа от сервера";
        public const string UnknownError = "Неизвестная ошибка при выполнении запроса";
    }
}