using FluentValidation;

namespace NockChat.Application.Rooms.Commands.JoinRoom
{
    /// <summary>
    /// Валидатор <see cref="JoinRoomCommand"/>
    /// </summary>
    public class JoinRoomCommandValidator : AbstractValidator<JoinRoomCommand>
    {
        /// <summary>
        /// Инициализирует правила валидации команды входа в комнату
        /// </summary>
        public JoinRoomCommandValidator()
        {
            RuleFor(x => x.AccessCode)
                .NotEmpty().WithMessage("Код доступа не может быть пустым")
                .Matches(@"^[A-Z0-9]{4}-[A-Z0-9]{4}$").WithMessage("Неверный формат кода доступа");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя не может быть пустым")
                .MaximumLength(50).WithMessage("Имя не может быть длиннее 50 символов");
        }
    }
}