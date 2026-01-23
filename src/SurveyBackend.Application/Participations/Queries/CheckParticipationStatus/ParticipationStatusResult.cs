namespace SurveyBackend.Application.Participations.Queries.CheckParticipationStatus;

public sealed record ParticipationStatusResult(
    bool HasParticipated,
    bool IsCompleted,
    DateTime? CompletedAt);
