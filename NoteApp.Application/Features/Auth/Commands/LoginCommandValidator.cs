using FluentValidation;

namespace NoteApp.Application.Features.Auth.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı gereklidir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre gereklidir.");
    }
}