using Serilog;

namespace NockChat.Api.Extensions
{
    /// <summary>
    /// Методы расширения для настройки логирования через Serilog
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Настраивает Serilog
        /// </summary>
        /// <param name="builder">Строитель веб-приложения</param>
        /// <returns>Тот же строитель для цепочки вызовов</returns>
        public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/log.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7
                ).CreateLogger();

            builder.Host.UseSerilog();
            return builder;
        }
    }
}