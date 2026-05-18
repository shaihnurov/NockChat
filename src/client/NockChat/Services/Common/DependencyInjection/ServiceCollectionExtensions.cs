using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Services.Common.DataStorage.Settings;
using NockChat.Services.Common.Extensions.Debounce;
using NockChat.Services.Common.Extensions.Navigations;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
using NockChat.Services.HTTP;
using NockChat.Services.HTTP.Network;
using NockChat.Services.HTTP.Options;
using NockChat.Services.HTTP.Requests;
using NockChat.ViewModels;

namespace NockChat.Services.Common.DependencyInjection
{
    /// <summary>
    /// Методы-расширения для регистрации сервисов в контейнере зависимостей
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрация сервисов с жизненным циклом Singleton
        /// </summary>
        public static IServiceCollection AddSingletonServices(this IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IAppUiState, AppUiState>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<INetworkService, NetworkService>();

            return services;
        }

        /// <summary>
        /// Регистрация сервисов с жизненным циклом Transient
        /// </summary>
        public static IServiceCollection AddTransientServices(this IServiceCollection services)
        {
            services.AddTransient<IDebounceDispatcher, DebounceDispatcher>();
            services.AddTransient<IRoomRequestsService, RoomRequestsService>();

            services.AddViewModel<MainViewModel>(ServiceLifetime.Transient);
            services.AddViewModel<HomeViewModel>(ServiceLifetime.Transient);

            return services;
        }

        /// <summary>
        /// Регистрация общих библиотечных сервисов
        /// </summary>
        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            services.AddHttpClient<IHttpService, HttpService>();
            services.Configure<HttpServiceOptions>(configuration.GetSection("HttpService"));

            return services;
        }

        private static void AddViewModel<T>(this IServiceCollection services, ServiceLifetime lifetime) where T : class
        {
            var descriptor = new ServiceDescriptor(typeof(T), typeof(T), lifetime);
            services.Add(descriptor);
            ViewModelLifetimeRegistry.Register(typeof(T), lifetime);
        }
    }
}