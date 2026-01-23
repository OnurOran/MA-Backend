using SurveyBackend.Application.Surveys.Commands.Create;

namespace SurveyBackend.Application.Surveys.Commands.Create;

public sealed class CreateSurveyCommandValidator : AbstractValidator<CreateSurveyCommand>
{
    public CreateSurveyCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        When(x => x.Questions is not null, () =>
        {
            RuleForEach(x => x.Questions!)
                .ChildRules(question =>
                {
                    question.RuleFor(q => q.Text)
                        .NotEmpty();
                });
        });
    }
}
