namespace SurveyBackend.Application.Participations.Commands.CompleteParticipation;

public sealed record CompleteParticipationCommand(int ParticipationId) : ICommand<bool>;
