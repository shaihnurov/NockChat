using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Application.Common.Behaviors;

namespace NockChat.Application.Extensions
{
    /// <summary>
    /// Методы расширения для регистрации служб Application-слоя
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(ApplicationServiceExtensions).Assembly);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

            services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}