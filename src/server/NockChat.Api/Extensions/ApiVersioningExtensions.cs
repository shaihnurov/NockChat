using Asp.Versioning;

namespace NockChat.Api.Extensions
{
    /// <summary>
    /// Методы расширения для настройки версионирования API
    /// </summary>
    public static class ApiVersioningExtensions
    {
        /// <summary>
        /// Регистрирует службы версионирования API с поддержкой URL-сегмента,
        /// заголовка и строки запроса в качестве источников версии
        /// </summary>
        /// <param name="services">Коллекция служб приложения</param>
        /// <returns>Та же коллекция служб для цепочки вызовов</returns>
        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("api-version"),
                    new QueryStringApiVersionReader("api-version")
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}