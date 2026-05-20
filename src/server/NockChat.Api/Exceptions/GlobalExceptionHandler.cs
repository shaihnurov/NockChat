using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NockChat.Application.Common.Exceptions;

namespace NockChat.Api.Exceptions
{
    /// <summary>
    /// Глобальный обработчик исключений. Перехватывает необработанные исключения
    /// и возвращает клиенту структурированный ответ в формате ProblemDetails (RFC 7807)
    /// </summary>
    /// <param name="logger">Логгер для записи</param>
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var (logLevel, status, title) = exception switch
            {
                ValidationException => (LogLevel.Warning, StatusCodes.Status400BadRequest, "Ошибка валидации"),
                NotFoundException => (LogLevel.Warning, StatusCodes.Status404NotFound, "Ресурс не найден"),
                ConflictException => (LogLevel.Warning, StatusCodes.Status409Conflict, "Конфликт данных"),
                ForbiddenException => (LogLevel.Warning, StatusCodes.Status403Forbidden, "Доступ запрещен"),
                _ => (LogLevel.Error, StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")
            };

            logger.Log(logLevel, exception, "Обработано исключение: {Message}", exception.Message);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
    }
}