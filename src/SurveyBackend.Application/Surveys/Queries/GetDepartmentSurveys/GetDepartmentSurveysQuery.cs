using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentSurveys;

public sealed record GetDepartmentSurveysQuery(int DepartmentId) : ICommand<IReadOnlyCollection<SurveyListItemDto>>, IDepartmentScopedCommand;
