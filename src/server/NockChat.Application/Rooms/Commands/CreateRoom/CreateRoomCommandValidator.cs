using FluentValidation;

namespace NockChat.Application.Rooms.Commands.CreateRoom
{
    public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
    {
        public CreateRoomCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название комнаты не может быть пустым")
                .MaximumLength(100).WithMessage("Название не может быть длиннее 100 символов");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя не может быть пустым")
                .MaximumLength(50).WithMessage("Имя не может быть длиннее 50 символов");
        }
    }
}