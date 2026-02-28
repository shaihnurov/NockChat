using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace NockChat.Services.Common.Extensions.Navigations
{
    public static class ViewModelLifetimeRegistry
    {
        private static readonly ConcurrentDictionary<Type, ServiceLifetime> _lifetimes = new();

        public static void Register(Type type, ServiceLifetime lifetime)
        {
            _lifetimes[type] = lifetime;
        }

        public static bool IsTransient(Type type)
        {
            return _lifetimes.TryGetValue(type, out var lifetime) && lifetime == ServiceLifetime.Transient;
        }
    }
}