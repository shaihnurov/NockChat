using FluentValidation;

namespace NockChat.Application.Messages.Queries
{
    /// <summary>
    /// Валидатор <see cref="GetMessagesQuery"/>
    /// </summary>
    public class GetMessagesQueryValidator : AbstractValidator<GetMessagesQuery>
    {
        /// <summary>
        /// Инициализирует правила валидации запроса получения сообщений
        /// </summary>
        public GetMessagesQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be >= 1");

            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");
        }
    }
}