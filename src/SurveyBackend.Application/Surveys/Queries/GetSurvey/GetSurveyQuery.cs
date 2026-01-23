using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurvey;

public sealed record GetSurveyQuery(int Id) : ICommand<SurveyDetailDto?>;
