namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyStats(
    int SurveyId,
    string Title,
    int DepartmentId,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? StartDate,
    DateTime? EndDate,
    int ParticipationCount);
