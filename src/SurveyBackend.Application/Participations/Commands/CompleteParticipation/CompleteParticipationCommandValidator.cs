namespace SurveyBackend.Application.Participations.Commands.CompleteParticipation;

public sealed class CompleteParticipationCommandValidator : AbstractValidator<CompleteParticipationCommand>
{
    public CompleteParticipationCommandValidator()
    {
        RuleFor(x => x.ParticipationId)
            .NotEmpty();
    }
}
