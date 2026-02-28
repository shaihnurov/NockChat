using Serilog.Core;
using Serilog.Events;

namespace NockChat.Services.Common.Extensions.Logger;

/// <summary>
/// Обогащает лог-событие, заменяя полное имя контекста источника (SourceContext)
/// на его сокращённую версию (только имя класса/типа без namespace)
/// </summary>
public class ShortSourceContextEnricher : ILogEventEnricher
{
    private const string SourceContextPropertyName = "SourceContext";

    /// <summary>
    /// Метод вызывается Serilog для обогащения каждого лог-события
    /// </summary>
    /// <param name="logEvent">Лог-событие, которое обогащается</param>
    /// <param name="propertyFactory">Фабрика для создания новых свойств</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out var sourceContextValue))
        {
            var fullName = sourceContextValue.ToString().Trim('\"');

            var lastDotIndex = fullName.LastIndexOf('.');

            var shortName = lastDotIndex == -1 ? fullName : fullName[(lastDotIndex + 1)..];

            var shortNameProperty = propertyFactory.CreateProperty(SourceContextPropertyName, shortName);

            logEvent.AddOrUpdateProperty(shortNameProperty);
        }
    }
}