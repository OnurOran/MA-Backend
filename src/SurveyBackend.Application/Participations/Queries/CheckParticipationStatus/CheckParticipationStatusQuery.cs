namespace SurveyBackend.Application.Participations.Queries.CheckParticipationStatus;

public sealed record CheckParticipationStatusQuery(int SurveyNumber, Guid? ExternalId) : ICommand<ParticipationStatusResult>;
