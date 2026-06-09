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
            RuleFor(x => x.Payload).NotNull().WithMessage("Payload не может быть пустым");

            RuleFor(x => x.Payload.Nonce)
                .NotEmpty().WithMessage("Nonce не может быть пустым")
                .Must(BeValidBase64).WithMessage("Nonce должен быть в формате Base64");

            RuleFor(x => x.Payload.Ciphertext)
                .NotEmpty().WithMessage("Ciphertext не может быть пустым")
                .Must(BeValidBase64).WithMessage("Ciphertext должен быть в формате Base64");

            RuleFor(x => x.Payload.RatchetPublicKey)
                .NotEmpty().WithMessage("RatchetPublicKey не может быть пустым")
                .Must(BeValidBase64).WithMessage("RatchetPublicKey должен быть в формате Base64");

            RuleFor(x => x.RoomId)
                .GreaterThan(0).WithMessage("Некорректный идентификатор комнаты");

            RuleFor(x => x.ChatUserId)
                .GreaterThan(0).WithMessage("Некорректный идентификатор пользователя");
        }

        private static bool BeValidBase64(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            Span<byte> buffer = stackalloc byte[512];
            return Convert.TryFromBase64String(value, buffer, out _);
        }
    }
}