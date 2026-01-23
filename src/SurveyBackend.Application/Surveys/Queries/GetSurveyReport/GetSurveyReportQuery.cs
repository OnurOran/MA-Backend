using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveyReport;

public sealed record GetSurveyReportQuery(int SurveyId, bool IncludePartialResponses = false) : ICommand<SurveyReportDto?>;

public sealed record GetParticipantResponseQuery(int SurveyId, int ParticipationId) : ICommand<ParticipantResponseDto?>;
