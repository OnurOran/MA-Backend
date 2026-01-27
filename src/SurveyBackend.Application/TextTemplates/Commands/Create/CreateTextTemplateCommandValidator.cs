using FluentValidation;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.TextTemplates.Commands.Create;

public sealed class CreateTextTemplateCommandValidator : AbstractValidator<CreateTextTemplateCommand>
{
    public CreateTextTemplateCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}
