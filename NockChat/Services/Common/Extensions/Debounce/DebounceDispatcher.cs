using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NockChat.Services.Common.Extensions.Debounce
{
    /// <summary>
    /// Мини‑дебаунсер: запускает <paramref name="action"/> только после того,
    /// как в течение <c>delayMS</c> миллисекунд больше не поступало вызовов <see cref="Debounce(Action)"/>
    /// </summary>
    public sealed class DebounceDispatcher(ILogger<DebounceDispatcher> logger) : IDebounceDispatcher
    {
        /// <summary>
        /// Текущий токен отмены; каждый вызов <see cref="Debounce"/> отменяет предыдущий
        /// </summary>
        private CancellationTokenSource? _cts;

        /// <summary>
        /// Контекст, на котором был создан экземпляр (UI‑поток Avalonia, если вызван из UI).
        /// Нужен, чтобы вернуться в UI‑поток после фоновой задержки
        /// </summary>
        private readonly SynchronizationContext? _constructedCtx = SynchronizationContext.Current;

        /// <summary>
        /// Если вызывающий не укажет delay — используется это значение (миллисекунды)
        /// </summary>
        public const int DEFAULT_DELAY_MS = 1500;

        /// <inheritdoc/>
        public void Debounce(Action action, int? delayMs = null)
        {
            int effectiveDelay = delayMs ?? DEFAULT_DELAY_MS;

            var old = Interlocked.Exchange(ref _cts, new());
            CancelAndDisposeSafe(old);

            var token = _cts.Token;
            var callCtx = SynchronizationContext.Current ?? _constructedCtx;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(effectiveDelay, token).ConfigureAwait(false);

                    if (token.IsCancellationRequested)
                        return;

                    if (callCtx != null)
                    {
                        callCtx.Post(_ =>
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Ошибка в debounced action.");
                            }
                        }, null);
                    }
                    else
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Ошибка в debounced action.");
                        }
                    }
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    // ожидаемая отмена
                }
                catch (OperationCanceledException oce)
                {
                    logger.LogDebug(oce, "OperationCanceledException в Debounce (не наш token).");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при выполнении действия в дебаунсере");
                }
            });
        }

        /// <summary>
        /// Безопасно отменяет и освобождает токен отмены
        /// </summary>
        private void CancelAndDisposeSafe(CancellationTokenSource? cts)
        {
            if (cts == null) return;
            try
            {
                cts.Cancel();
                cts.Dispose();
            }
            catch (ObjectDisposedException ex)
            {
                logger.LogDebug(ex, "CancellationTokenSource уже был освобождён.");
            }
            catch (AggregateException ex)
            {
                logger.LogWarning(ex, "Ошибка при отмене задач через CancellationTokenSource.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Неожиданная ошибка при отмене/освобождении CancellationTokenSource.");
            }
        }

        /// <inheritdoc/>
        public void Clear() => CancelAndDisposeSafe(Interlocked.Exchange(ref _cts, null));

        public void Dispose() => Clear();
    }
}