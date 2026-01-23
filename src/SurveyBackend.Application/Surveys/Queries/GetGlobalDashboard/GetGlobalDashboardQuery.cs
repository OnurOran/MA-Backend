using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetGlobalDashboard;

public sealed record GetGlobalDashboardQuery() : ICommand<GlobalDashboardDto>;
