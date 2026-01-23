using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetGlobalDashboard;

public sealed class GetGlobalDashboardQueryHandler : ICommandHandler<GetGlobalDashboardQuery, GlobalDashboardDto>
{
    private readonly ISurveyRepository _surveyRepository;

    public GetGlobalDashboardQueryHandler(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task<GlobalDashboardDto> HandleAsync(GetGlobalDashboardQuery request, CancellationToken cancellationToken)
    {
        var surveyStats = await _surveyRepository.GetSurveyStatsAsync(cancellationToken);
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

        return new GlobalDashboardDto(
            totalSurveys,
            activeSurveys,
            totalParticipations,
            items);
    }
}
