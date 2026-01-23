using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentDashboard;

public sealed class GetDepartmentDashboardQueryHandler : ICommandHandler<GetDepartmentDashboardQuery, DepartmentDashboardDto>
{
    private readonly ISurveyRepository _surveyRepository;

    public GetDepartmentDashboardQueryHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<DepartmentDashboardDto> HandleAsync(GetDepartmentDashboardQuery request, CancellationToken cancellationToken)
    {
        var surveyStats = await _surveyRepository.GetSurveyStatsByDepartmentAsync(request.DepartmentId, cancellationToken);
        var now = DateTime.Now;

        var totalSurveys = surveyStats.Count;
        var activeSurveys = surveyStats.Count(s =>
            s.IsActive &&
            s.StartDate.HasValue &&
            s.EndDate.HasValue &&
            s.StartDate.Value <= now &&
            s.EndDate.Value >= now);
        var totalParticipations = surveyStats.Sum(s => s.ParticipationCount);

        var items = surveyStats
            .Select(s =>
            {
                var isCurrentlyActive = s.IsActive &&
                    s.StartDate.HasValue &&
                    s.EndDate.HasValue &&
                    s.StartDate.Value <= now &&
                    s.EndDate.Value >= now;
                return new SurveyDashboardItemDto(s.SurveyId, s.Title, isCurrentlyActive, s.CreatedAt, s.ParticipationCount);
            })
            .ToList();

        return new DepartmentDashboardDto(
            request.DepartmentId,
            totalSurveys,
            activeSurveys,
            totalParticipations,
            items);
    }
}
