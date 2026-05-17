using CommunityToolkit.Mvvm.ComponentModel;

namespace NockChat.Services.Common.UI
{
    /// <summary>
    /// Реализация глобального состояния UI приложения
    /// </summary>
    public partial class AppUiState : ObservableObject, IAppUiState
    {
        /// <inheritdoc/>
        [ObservableProperty]
        private bool _hasError;

        /// <inheritdoc/>
        [ObservableProperty]
        private bool _hasConnection;

        /// <inheritdoc/>
        [ObservableProperty]
        private string? _statusBarMessage;

        /// <inheritdoc/>
        [ObservableProperty]
        public string? _titlePage;

        /// <inheritdoc/>
        [ObservableProperty]
        public bool _isActiveToggleMenu;
    }
}