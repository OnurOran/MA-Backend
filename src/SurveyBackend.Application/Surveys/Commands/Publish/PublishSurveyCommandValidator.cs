namespace SurveyBackend.Application.Surveys.Commands.Publish;

public sealed class PublishSurveyCommandValidator : AbstractValidator<PublishSurveyCommand>
{
    public PublishSurveyCommandValidator()
    {
        RuleFor(x => x.SurveyId)
            .GreaterThan(0);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate!.Value)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
