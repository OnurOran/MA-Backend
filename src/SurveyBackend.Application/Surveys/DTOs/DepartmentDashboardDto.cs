namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record DepartmentDashboardDto(
    int DepartmentId,
    int TotalSurveys,
    int ActiveSurveys,
    int TotalParticipations,
    IReadOnlyCollection<SurveyDashboardItemDto> Surveys);
