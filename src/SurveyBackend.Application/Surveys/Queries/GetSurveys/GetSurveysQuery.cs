using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveys;

public sealed record GetSurveysQuery() : ICommand<IReadOnlyCollection<SurveyListItemDto>>;
