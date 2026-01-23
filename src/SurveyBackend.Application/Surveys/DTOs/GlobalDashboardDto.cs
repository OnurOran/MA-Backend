namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record GlobalDashboardDto(
    int TotalSurveys,
    int ActiveSurveys,
    int TotalParticipations,
    IReadOnlyCollection<SurveyDashboardItemDto> Surveys);
