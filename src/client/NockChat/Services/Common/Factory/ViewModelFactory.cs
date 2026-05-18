using System;
using Microsoft.Extensions.DependencyInjection;

namespace NockChat.Services.Common.Factory
{
    /// <summary>
    /// Реализация фабрики ViewModel, использующая DI-контейнер для создания экземпляров
    /// </summary>
    /// <typeparam name="T">Тип создаваемого ViewModel</typeparam>
    /// <remarks>
    /// Инициализирует новый экземпляр <see cref="ViewModelFactory{T}"/>.
    /// </remarks>
    /// <param name="serviceProvider">Провайдер служб, используемый для создания объектов</param>
    public class ViewModelFactory<T>(IServiceProvider serviceProvider) : IViewModelFactory<T> where T : class
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        /// <inheritdoc />
        public T Create(params object[] parameters)
        {
            // Создает экземпляр ViewModel, используя DI-контейнер и переданные параметры
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider, parameters);
        }
    }
}