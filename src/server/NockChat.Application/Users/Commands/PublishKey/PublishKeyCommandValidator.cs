using FluentValidation;

namespace NockChat.Application.Users.Commands.PublishKey
{
    /// <summary>
    /// Валидатор <see cref="PublishKeyCommand"/>
    /// </summary>
    public class PublishKeyCommandValidator : AbstractValidator<PublishKeyCommand>
    {
        public PublishKeyCommandValidator()
        {
            RuleFor(x => x.EphemeralPublicKey)
                .NotEmpty().WithMessage("Публичный ключ не может быть пустым")
                .Must(BeValidBase64).WithMessage("Публичный ключ должен быть в формате Base64")
                .Length(44).WithMessage("Некорректная длина публичного ключа");
        }

        private static bool BeValidBase64(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            Span<byte> buffer = stackalloc byte[64];
            return Convert.TryFromBase64String(value, buffer, out _);
        }
    }
}