using FluentValidation;
using SurveyBackend.Application.Modules.Auth.Commands.Refresh;

namespace SurveyBackend.Application.Modules.Auth.Validators;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
