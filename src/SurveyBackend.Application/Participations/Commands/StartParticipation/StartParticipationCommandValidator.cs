namespace SurveyBackend.Application.Participations.Commands.StartParticipation;

public sealed class StartParticipationCommandValidator : AbstractValidator<StartParticipationCommand>
{
    public StartParticipationCommandValidator()
    {
        RuleFor(x => x.SurveyNumber)
            .GreaterThan(0)
            .WithMessage("SurveyNumber must be greater than 0");
    }
}
