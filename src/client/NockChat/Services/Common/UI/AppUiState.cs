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
        public partial string? TitlePage { get; set; }

        /// <inheritdoc/>
        [ObservableProperty]
        public partial bool IsVisibleMenu { get; set; }
    }
}