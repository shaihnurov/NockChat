using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NockChat.Models.Messages;
using NockChat.Models.Rooms;
using NockChat.Models.Sessions;
using NockChat.Services.Common.Exceptions;
using NockChat.Services.Common.Factory;
using NockChat.Services.Common.Navigations;
using NockChat.Services.Common.Notifications;
using NockChat.Services.Common.UI;
using NockChat.Services.Crypto;
using NockChat.Services.HTTP.Requests.Messages;
using NockChat.Services.HTTP.SignalR;
using NockChat.ViewModels.Dialogs;
using NockChat.Views.Dialogs;
using Ursa.Controls;

namespace NockChat.ViewModels
{
    /// <summary>
    /// ViewModel страницы чата
    /// </summary>
    public partial class ChatViewModel(ISignalRService signalRService, IMessageRequestsService messageService, IChatCryptoService cryptoService, IServiceProvider serviceProvider,
        INotificationService notificationService, INavigationService navigationService, IAppUiState appUiState, RoomSession session) : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Список сообщений текущей комнаты
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<MessageModel> Messages { get; set; } = [];

        /// <summary>
        /// Текст набираемого сообщения
        /// </summary>
        [ObservableProperty]
        public partial string MessageText { get; set; } = string.Empty;

        /// <summary>
        /// Указывает, выполняется ли подключение к комнате
        /// </summary>
        [ObservableProperty]
        public partial bool IsConnecting { get; set; }

        /// <summary>
        /// Вызывается при отправке собственного сообщения
        /// </summary>
        public event Action? OwnMessageSent;

        /// <summary>
        /// Название текущей комнаты
        /// </summary>
        public string RoomName => session.RoomName;

        private RoomCryptoManager? _cryptoManager;
        #endregion

        #region Methods
        /// <summary>
        /// Подключается к комнате через SignalR и загружает историю сообщений
        /// </summary>
        public override async Task Initialize()
        {
            try
            {
                appUiState.IsVisibleMenu = false;
                IsConnecting = true;

                _cryptoManager = new RoomCryptoManager(cryptoService, session.RoomId);

                await signalRService.ConnectAsync(session.Token);

                SubscribeToSignalREvents();

                var ourPublicKeyBase64 = Convert.ToBase64String(_cryptoManager.OurPublicKey);
                await signalRService.PublishKeyAsync(ourPublicKeyBase64);

                var result = await messageService.GetMessagesAsync(session.Token, 1, 100);
                var historyMessages = result.Items.Select(m =>
                {
                    m.Text = "[сообщение недоступно — история зашифрована]";
                    return m;
                });

                Messages = new ObservableCollection<MessageModel>(historyMessages);
            }
            catch (ServerException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (NetworkException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            catch (SignalRException ex)
            {
                notificationService.ShowError(ex.Message);
            }
            finally
            {
                IsConnecting = false;
            }
        }

        /// <summary>
        /// Подписывается на все SignalR события комнаты
        /// </summary>
        private void SubscribeToSignalREvents()
        {
            signalRService.OnReceiveRoomKeys(OnReceiveRoomKeys);
            signalRService.OnParticipantKeyPublished(OnParticipantKeyPublished);
            signalRService.OnMessageReceived(OnMessageReceived);

            signalRService.OnUserJoined(username =>
                Dispatcher.UIThread.Post(() => Messages.Add(CreateSystemMessage($"{username} вошёл в комнату"))));

            signalRService.OnUserLeft(username =>
                Dispatcher.UIThread.Post(() => Messages.Add(CreateSystemMessage($"{username} покинул комнату"))));
        }

        /// <summary>
        /// Сервер вернул ключи всех участников уже находящихся в комнате
        /// Устанавливаем крипто-сессию с каждым
        /// </summary>
        private void OnReceiveRoomKeys(IReadOnlyList<RoomKeyModel> keys)
        {
            if (_cryptoManager is null)
                return;

            foreach (var key in keys)
            {
                try
                {
                    var theirPublicKey = Convert.FromBase64String(key.EphemeralPublicKey);
                    _cryptoManager.AddPeer(key.ChatUserId, theirPublicKey);
                }
                catch (Exception)
                {
                    notificationService.ShowError($"Не удалось установить сессию с {key.Username}");
                }
            }
        }

        /// <summary>
        /// Новый участник опубликовал свой ключ — устанавливаем с ним крипто-сессию
        /// </summary>
        private void OnParticipantKeyPublished(RoomKeyModel key)
        {
            if (_cryptoManager is null)
                return;

            try
            {
                var theirPublicKey = Convert.FromBase64String(key.EphemeralPublicKey);
                _cryptoManager.AddPeer(key.ChatUserId, theirPublicKey);
            }
            catch (Exception)
            {
                notificationService.ShowError($"Не удалось установить сессию с {key.Username}");
            }
        }

        /// <summary>
        /// Получено зашифрованное сообщение — дешифруем и добавляем в список
        /// </summary>
        private void OnMessageReceived(MessageModel msg)
        {
            if (_cryptoManager is null || msg.EncryptedPayload is null)
                return;

            try
            {
                if (_cryptoManager.HasSession(msg.SenderId))
                    msg.Text = _cryptoManager.DecryptFrom(msg.SenderId, msg.EncryptedPayload);
                else
                    msg.Text = "[сообщение недоступно — нет крипто-сессии]";
            }
            catch (Exception)
            {
                msg.Text = "[не удалось расшифровать сообщение]";
            }

            Dispatcher.UIThread.Post(() => Messages.Add(msg));
        }

        /// <summary>
        /// Отправляет сообщение через SignalR
        /// Применяет оптимистичное обновление — сообщение добавляется в список до подтверждения сервера
        /// и удаляется обратно в случае ошибки
        /// </summary>
        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText) || _cryptoManager is null)
                return;

            var text = MessageText;
            MessageText = string.Empty;

            var optimisticMessage = new MessageModel
            {
                Text = text,
                Username = session.Username,
                SentAt = DateTimeOffset.UtcNow,
                IsOwn = true
            };

            Messages.Add(optimisticMessage);
            OwnMessageSent?.Invoke();

            try
            {
                foreach (var (peerId, encrypted) in _cryptoManager.EncryptForAll(text))
                    await signalRService.SendMessageAsync(encrypted);
            }
            catch (SignalRException ex)
            {
                Messages.Remove(optimisticMessage);
                MessageText = text;
                notificationService.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Отключается от SignalR и возвращает пользователя к списку комнат
        /// </summary>
        [RelayCommand]
        private async Task LeaveRoom()
        {
            _cryptoManager?.Dispose();
            _cryptoManager = null;

            await signalRService.DisconnectAsync();
            await navigationService.RequestNavigation<ChatListViewModel>();
            appUiState.IsVisibleMenu = true;
        }

        /// <summary>
        /// Открывает диалог настроек комнаты
        /// </summary>
        [RelayCommand]
        private async Task Options()
        {
            try
            {
                var optionsVm = serviceProvider.GetRequiredService<IViewModelFactory<ChatOptionsDialogViewModel>>().Create(session);
                await optionsVm.Initialize();
                await OverlayDialog.ShowCustomAsync<ChatOptionsDialogView, ChatOptionsDialogViewModel, bool>(vm: optionsVm);
            }
            catch (Exception)
            {
                notificationService.ShowError("Не удалось открыть настройки");
            }
        }

        private static MessageModel CreateSystemMessage(string text) => new()
        {
            Text = text,
            Username = "Система",
            SentAt = DateTimeOffset.UtcNow,
            IsOwn = false
        };
        #endregion
    }
}