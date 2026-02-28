using System;
using Serilog.Core;
using Serilog.Events;

namespace NockChat.Services.Common.Extensions.Logger;

/// <summary>
/// Обогащает события логирования дополнительной информацией об исключениях, если они присутствуют
/// </summary>
/// <remarks>
/// Добавляет структурированные свойства в <see cref="LogEvent"/>,
/// которые можно использовать для фильтрации, поиска и агрегации логов
/// </remarks>
public class ExceptionEnricher : ILogEventEnricher
{
    /// <summary>
    /// Обогащает событие логирования данными об исключении
    /// </summary>
    /// <param name="logEvent">Событие логирования Serilog</param>
    /// <param name="propertyFactory">Фабрика для создания свойств лог-события</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Exception == null)
            return;

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("ErrorId", Guid.NewGuid()));

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("ErrorType", logEvent.Exception.GetType().Name));

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("ErrorMessage", logEvent.Exception.Message));
    }
}