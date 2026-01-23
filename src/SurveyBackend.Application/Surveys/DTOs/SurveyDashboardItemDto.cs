namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyDashboardItemDto(
    int SurveyId,
    string Title,
    bool IsActive,
    DateTime CreatedAt,
    int ParticipationCount);
