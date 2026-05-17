using System;
using System.Threading.Tasks;
using NockChat.ViewModels;

namespace NockChat.Services.Common.Navigations
{
    /// <summary>
    /// Интерфейс для навигации между страницами
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Action для уведомления подписчиков о смене страницы
        /// </summary>
        event Action<ViewModelBase>? PageChanged;

        /// <summary>
        /// Метод для смены текущего представления
        /// </summary>
        Task RequestNavigation<T>() where T : ViewModelBase;

        /// <summary>
        /// Позволяет сменить страницу
        /// </summary>
        /// <param name="viewModelType">Тип страницы, которую необходимо отобразить</param>
        Task NavigateTo(Type viewModelType);

        /// <summary>
        /// Позволяет сменить страницу
        /// </summary>
        /// <param name="viewModel">Cтраница, которая реализует ViewModelBase</param>
        Task NavigateTo(ViewModelBase viewModel);

        /// <summary>
        /// Позволяет получить страницу через DI
        /// </summary>
        /// <param name="viewModelType">Страница</param>
        /// <returns>Страница готовая к отображению</returns>
        ViewModelBase GetViewModel(Type viewModelType);
    }
}