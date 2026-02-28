using System;

namespace NockChat.Services.Common.Extensions.Debounce
{
    /// <summary>
    /// Контракт для дебаунсера: позволяет вызвать отложенное действие, очистить ожидающее действие и корректно освободить ресурсы
    /// </summary>
    public interface IDebounceDispatcher : IDisposable
    {
        /// <summary>
        /// Запускает/перезапускает дебаунс. Если за время delayMs (мс) не придёт новый вызов,
        /// будет выполнено action. Если delayMs == null — используется DEFAULT_DELAY_MS
        /// </summary>
        void Debounce(Action action, int? delayMs = null);

        /// <summary>
        /// Отменяет текущее отложенное действие и снимает ссылку на внутренний CTS
        /// </summary>
        void Clear();
    }
}
