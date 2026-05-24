using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace NockChat.Services.Common.DependencyInjection
{
    /// <summary>
    /// Реестр времён жизни ViewModel, зарегистрированных в DI-контейнере
    /// Используется для принятия решения об утилизации ViewModel при навигации
    /// </summary>
    public static class ViewModelLifetimeRegistry
    {
        /// <summary>
        /// Словарь соответствий типа ViewModel и её времени жизни в DI-контейнере
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ServiceLifetime> _lifetimes = new();

        /// <summary>
        /// Регистрирует время жизни ViewModel
        /// </summary>
        /// <param name="type">Тип ViewModel</param>
        /// <param name="lifetime">Время жизни в DI-контейнере</param>
        public static void Register(Type type, ServiceLifetime lifetime)
        {
            _lifetimes[type] = lifetime;
        }

        /// <summary>
        /// Проверяет, зарегистрирована ли ViewModel как <see cref="ServiceLifetime.Transient"/>
        /// Transient-объекты должны явно утилизироваться при уходе со страницы,
        /// так как DI-контейнер не управляет их жизненным циклом после создания
        /// </summary>
        /// <param name="type">Тип ViewModel</param>
        /// <returns><c>true</c> если ViewModel является Transient</returns>
        public static bool IsTransient(Type type)
        {
            return _lifetimes.TryGetValue(type, out var lifetime) && lifetime == ServiceLifetime.Transient;
        }
    }
}