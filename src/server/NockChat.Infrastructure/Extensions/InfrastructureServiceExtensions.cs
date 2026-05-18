using Microsoft.Extensions.DependencyInjection;
using NockChat.Application.Common.Interfaces;
using NockChat.Infrastructure.Services;

namespace NockChat.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<IChatNotificationService, ChatNotificationService>();

            return services;
        }
    }
}