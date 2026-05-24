using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Services.Attributes;
using NockChat.Services.Common.DependencyInjection;
using NockChat.Services.Common.Extensions;
using NockChat.Services.Common.UI;
using NockChat.ViewModels;

namespace NockChat.Services.Common.Navigations
{
    /// <summary>
    /// Сервис навигации между страницами приложения
    /// </summary>
    public partial class NavigationService(IServiceProvider serviceProvider, IAppUiState appUiState) : ObservableObject, INavigationService
    {
        /// <summary>
        /// Кэш атрибутов конфигурации страниц по типу ViewModel
        /// Используется для ускорения доступа к информации о заголовке и активности меню
        /// </summary>
        private static readonly Dictionary<Type, ViewAttribute> _viewAttributeConfigs = [];

        /// <inheritdoc/>
        public event Action<ViewModelBase>? PageChanged;

        private ViewModelBase? _currentViewModel;

        #region Методы навигации
        /// <inheritdoc/>
        public async Task RequestNavigation<T>() where T : ViewModelBase => await NavigateTo(typeof(T));

        /// <inheritdoc/>
        public ViewModelBase GetViewModel(Type viewModelType) => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType);

        /// <inheritdoc/>
        public async Task NavigateTo(Type viewModelType)
        {
            if (_currentViewModel != null)
            {
                if (_currentViewModel is IEventCleaning cleaner)
                    cleaner.CleanEvent();

                if (ViewModelLifetimeRegistry.IsTransient(_currentViewModel.GetType()))
                    if (_currentViewModel is IDisposable disposable)
                        disposable.Dispose();
            }

            if (!_viewAttributeConfigs.TryGetValue(viewModelType, out var config))
            {
                config = viewModelType.GetCustomAttribute<ViewAttribute>() ?? new ViewAttribute(viewModelType.Name);
                _viewAttributeConfigs[viewModelType] = config;
            }

            appUiState.TitlePage = config.Title;

            var vm = GetViewModel(viewModelType);
            await vm.Initialize();

            _currentViewModel = vm;
            PageChanged?.Invoke(vm);
        }

        /// <inheritdoc/>
        public async Task NavigateTo(ViewModelBase viewModel)
        {
            Type viewModelType = viewModel.GetType();

            if (_currentViewModel != null)
            {
                if (_currentViewModel is IEventCleaning cleaner)
                    cleaner.CleanEvent();

                if (ViewModelLifetimeRegistry.IsTransient(_currentViewModel.GetType()))
                    if (_currentViewModel is IDisposable disposable)
                        disposable.Dispose();
            }

            if (!_viewAttributeConfigs.TryGetValue(viewModelType, out var config))
            {
                config = viewModelType.GetCustomAttribute<ViewAttribute>() ?? new ViewAttribute(viewModelType.Name);
                _viewAttributeConfigs[viewModelType] = config;
            }

            appUiState.TitlePage = config.Title;
            await viewModel.Initialize();

            _currentViewModel = viewModel;
            PageChanged?.Invoke(viewModel);
        }
        #endregion
    }
}