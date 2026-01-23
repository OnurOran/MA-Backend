using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Participations.Queries.CheckParticipationStatus;

public sealed class CheckParticipationStatusQueryHandler : ICommandHandler<CheckParticipationStatusQuery, ParticipationStatusResult>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ICurrentUserService _currentUserService;

    public CheckParticipationStatusQueryHandler(
        ISurveyRepository surveyRepository,
        IParticipantRepository participantRepository,
        IParticipationRepository participationRepository,
        ICurrentUserService currentUserService)
    {
        _surveyRepository = surveyRepository;
        _participantRepository = participantRepository;
        _participationRepository = participationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ParticipationStatusResult> HandleAsync(CheckParticipationStatusQuery request, CancellationToken cancellationToken)
    {

        var survey = await _surveyRepository.GetByIdAsync(request.SurveyNumber, cancellationToken);
        if (survey is null)
        {
            throw new InvalidOperationException("Anket bulunamadı.");
        }

        Domain.Surveys.Participant? participant = null;

        if (_currentUserService.IsAuthenticated)
        {
            var username = _currentUserService.Username;
            if (!string.IsNullOrWhiteSpace(username))
            {
                participant = await _participantRepository.GetByLdapUsernameAsync(username, cancellationToken);
            }
        }
        else
        {
            if (request.ExternalId.HasValue)
            {
                participant = await _participantRepository.GetByExternalIdAsync(request.ExternalId.Value, cancellationToken);
            }
        }

        if (participant is null)
        {
            return new ParticipationStatusResult(
                HasParticipated: false,
                IsCompleted: false,
                CompletedAt: null);
        }

        var participation = await _participationRepository.GetBySurveyAndParticipantAsync(
            survey.Id,
            participant.Id,
            cancellationToken);

        if (participation is null)
        {
            return new ParticipationStatusResult(
                HasParticipated: false,
                IsCompleted: false,
                CompletedAt: null);
        }

        var isCompleted = participation.CompletedAt.HasValue;
        return new ParticipationStatusResult(
            HasParticipated: true,
            IsCompleted: isCompleted,
            CompletedAt: participation.CompletedAt);
    }
}
