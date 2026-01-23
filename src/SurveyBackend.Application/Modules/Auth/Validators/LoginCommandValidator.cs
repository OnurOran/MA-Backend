using FluentValidation;
using SurveyBackend.Application.Modules.Auth.Commands.Login;

namespace SurveyBackend.Application.Modules.Auth.Validators;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
