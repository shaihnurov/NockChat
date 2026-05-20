using FluentValidation;

namespace NockChat.Application.Messages.Commands.SendMessage
{
    /// <summary>
    /// Валидатор <see cref="SendMessageCommand"/>
    /// Проверяет корректность текста сообщения, идентификатора комнаты и пользователя
    /// </summary>
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        /// <summary>
        /// Инициализирует правила валидации команды отправки сообщения
        /// </summary>
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Текст сообщения не может быть пустым")
                .MaximumLength(4000).WithMessage("Текст не может быть длиннее 4000 символов");

            RuleFor(x => x.RoomId)
                .GreaterThan(0).WithMessage("Некорректный идентификатор комнаты");

            RuleFor(x => x.ChatUserId)
                .GreaterThan(0).WithMessage("Некорректный идентификатор пользователя");
        }
    }
}