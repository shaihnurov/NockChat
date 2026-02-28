using System;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Services.Common.DataStorage.Settings;
using NockChat.Services.Common.DependencyInjection;
using NockChat.Services.Common.Extensions.Debounce;
using NockChat.Services.Common.Extensions.Navigations;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
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

            return services;
        }

        /// <summary>
        /// Регистрация сервисов с жизненным циклом Transient
        /// </summary>
        public static IServiceCollection AddTransientServices(this IServiceCollection services)
        {
            services.AddTransient<IDebounceDispatcher, DebounceDispatcher>();

            services.AddViewModel<MainViewModel>(ServiceLifetime.Transient);

            return services;
        }

        /// <summary>
        /// Регистрация общих библиотечных сервисов
        /// </summary>
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {

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