using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Application.Modules.Authorization.Validators;

public sealed class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.RoleId).GreaterThan(0);
    }
}
