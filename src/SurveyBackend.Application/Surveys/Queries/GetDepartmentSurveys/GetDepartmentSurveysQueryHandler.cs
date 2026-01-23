using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetDepartmentSurveys;

public sealed class GetDepartmentSurveysQueryHandler : ICommandHandler<GetDepartmentSurveysQuery, IReadOnlyCollection<SurveyListItemDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;

    public GetDepartmentSurveysQueryHandler(ISurveyRepository surveyRepository, IUserRepository userRepository)
    {
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<SurveyListItemDto>> HandleAsync(GetDepartmentSurveysQuery request, CancellationToken cancellationToken)
    {
        var surveys = await _surveyRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

        var creatorCache = new Dictionary<int, string>();
        var result = new List<SurveyListItemDto>(surveys.Count);

        foreach (var survey in surveys)
        {
            var createdBy = await ResolveCreatorAsync(survey.CreateEmployeeId, creatorCache, cancellationToken);

            result.Add(new SurveyListItemDto(
                survey.Id,
                survey.Id,
                survey.Slug,
                survey.Title,
                survey.Description,
                survey.DepartmentId,
                survey.AccessType,
                survey.IsPublished,
                survey.CreateDate,
                survey.StartDate,
                survey.EndDate,
                createdBy));
        }

        return result;
    }

    private async Task<string> ResolveCreatorAsync(int? userId, Dictionary<int, string> cache, CancellationToken cancellationToken)
    {
        if (!userId.HasValue)
        {
            return "Bilinmeyen";
        }

        if (cache.TryGetValue(userId.Value, out var cached))
        {
            return cached;
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        var resolved = user?.Username ?? $"User#{userId}";
        cache[userId.Value] = resolved;
        return resolved;
    }
}
