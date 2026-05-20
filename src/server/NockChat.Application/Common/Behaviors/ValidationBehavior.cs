using FluentValidation;
using MediatR;

namespace NockChat.Application.Common.Behaviors
{
    /// <summary>
    /// Поведение MediatR pipeline для автоматической валидации входящих запросов
    /// Запускается перед обработчиком и выбрасывает <see cref="ValidationException"/> при наличии ошибок валидации
    /// </summary>
    /// <typeparam name="TRequest">Тип входящего запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа обработчика</typeparam>
    /// <param name="validators">Коллекция валидаторов для данного типа запроса</param>
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        /// <summary>
        /// Выполняет валидацию запроса всеми зарегистрированными валидаторами
        /// Если валидаторов нет — передаёт запрос следующему обработчику без проверок
        /// </summary>
        /// <param name="request">Входящий запрос</param>
        /// <param name="next">Делегат следующего обработчика в pipeline</param>
        /// <param name="ct">Токен</param>
        /// <returns>Ответ следующего обработчика</returns>
        /// <exception cref="ValidationException">Выбрасывается при наличии ошибок валидации</exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (!validators.Any())
                return await next(ct);

            var context = new ValidationContext<TRequest>(request);

            var failures = validators.Select(v => v.Validate(context)).SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);

            return await next(ct);
        }
    }
}