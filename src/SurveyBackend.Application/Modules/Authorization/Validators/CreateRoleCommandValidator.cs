using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Application.Modules.Authorization.Validators;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Permissions).NotEmpty();
    }
}
