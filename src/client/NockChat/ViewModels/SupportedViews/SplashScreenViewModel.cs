using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NockChat.ViewModels.SupportedViews
{
    /// <summary>
    /// ViewModel - отвечающая за SplashScreen приложения
    /// </summary>
    public partial class SplashScreenViewModel : ViewModelBase
    {
        /// <summary>
        /// Основной текст для вывода информации при инициализация клиента
        /// </summary>
        [ObservableProperty]
        private string _startUpMessage = string.Empty;

        private readonly CancellationTokenSource _cts = new();
        public CancellationToken CancellationToken => _cts.Token;
    }
}