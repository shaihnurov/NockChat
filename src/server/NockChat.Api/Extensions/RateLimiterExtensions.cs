using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace NockChat.Api.Extensions
{
    /// <summary>
    /// Методы расширения для настройки ограничения частоты запросов (rate limiting)
    /// </summary>
    public static class RateLimiterExtensions
    {
        /// <summary>
        /// Регистрирует глобальный скользящий лимит запросов (100 в минуту на IP-адрес),
        /// а также именованный лимит <c>join-room</c> (5 в минуту) для эндпоинта входа в комнату
        /// При превышении лимита возвращается статус 429 с заголовком <c>Retry-After</c>
        /// </summary>
        /// <param name="services">Коллекция служб приложения</param>
        /// <returns>Та же коллекция служб для цепочки вызовов</returns>
        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = OnRejectedHandler;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "Guest";
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: partitionKey,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            PermitLimit = 100,
                            QueueLimit = 0
                        });
                });

                options.AddSlidingWindowLimiter("join-room", o =>
                {
                    o.Window = TimeSpan.FromMinutes(1);
                    o.SegmentsPerWindow = 6;
                    o.PermitLimit = 5;
                    o.QueueLimit = 0;
                });
            });

            return services;
        }

        /// <summary>
        /// Обработчик отклонённых запросов. Формирует JSON-ответ с сообщением об ошибке
        /// и временем ожидания до следующей попытки в заголовке <c>Retry-After</c>
        /// </summary>
        /// <param name="context">Контекст отклонённого запроса</param>
        /// <param name="ct">Токен</param>
        private static async ValueTask OnRejectedHandler(OnRejectedContext context, CancellationToken ct)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";

            var retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) ? (int)retryAfter.TotalSeconds : 60;

            context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString();

            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Слишком много запросов. Попробуйте позже.",
                retryAfterSeconds
            }, ct);
        }
    }
}