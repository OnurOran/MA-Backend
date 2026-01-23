using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Surveys.Commands.Publish;

public sealed class PublishSurveyCommandHandler : ICommandHandler<PublishSurveyCommand, bool>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAuthorizationService _authorizationService;

    public PublishSurveyCommandHandler(ISurveyRepository surveyRepository, IAuthorizationService authorizationService)
    {
        _surveyRepository = surveyRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> HandleAsync(PublishSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

        survey.Publish(request.StartDate, request.EndDate);

        await _surveyRepository.UpdateAsync(survey, cancellationToken);
        return true;
    }
}
