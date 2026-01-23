namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record PublishSurveyRequest(DateTime? StartDate, DateTime? EndDate);
