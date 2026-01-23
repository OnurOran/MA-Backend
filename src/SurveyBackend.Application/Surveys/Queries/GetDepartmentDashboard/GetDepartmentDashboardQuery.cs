using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentDashboard;

public sealed record GetDepartmentDashboardQuery(int DepartmentId) : ICommand<DepartmentDashboardDto>, IDepartmentScopedCommand;
