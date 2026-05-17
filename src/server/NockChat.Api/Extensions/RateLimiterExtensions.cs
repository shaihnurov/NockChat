using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace NockChat.Api.Extensions
{
    public static class RateLimiterExtensions
    {
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