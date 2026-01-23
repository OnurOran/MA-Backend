using FluentValidation;
using SurveyBackend.Application.Modules.Auth.DTOs;

namespace SurveyBackend.Application.Modules.Auth.Validators;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .When(x => x.RefreshToken is not null);
    }
}
