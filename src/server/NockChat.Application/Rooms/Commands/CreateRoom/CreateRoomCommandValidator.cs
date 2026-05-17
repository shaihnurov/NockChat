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
        }
    }
}