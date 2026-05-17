using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace NockChat.Application.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

            services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);

            return services;
        }
    }
}