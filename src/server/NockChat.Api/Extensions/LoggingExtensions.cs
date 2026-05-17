using Serilog;

namespace NockChat.Api.Extensions
{
    public static class LoggingExtensions
    {
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