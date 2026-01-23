namespace SurveyBackend.Application.Surveys.Commands.AddQuestion;

public sealed class AddQuestionCommandValidator : AbstractValidator<AddQuestionCommand>
{
    public AddQuestionCommandValidator()
    {
        RuleFor(x => x.SurveyId)
            .NotEmpty();

        RuleFor(x => x.Question)
            .NotNull();

        When(x => x.Question is not null, () =>
        {
            RuleFor(x => x.Question!.Text)
                .NotEmpty();

            When(x => x.Question!.Options is not null, () =>
            {
                RuleForEach(x => x.Question!.Options!)
                    .ChildRules(option =>
                    {
                        option.RuleFor(o => o.Text)
                            .NotEmpty();
                    });
            });
        });
    }
}
