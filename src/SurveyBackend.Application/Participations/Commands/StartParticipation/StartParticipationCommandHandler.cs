using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Participations.Commands.StartParticipation;

public sealed class StartParticipationCommandHandler : ICommandHandler<StartParticipationCommand, int>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ICurrentUserService _currentUserService;

    public StartParticipationCommandHandler(
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

    public async Task<int> HandleAsync(StartParticipationCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyNumber, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        if (!IsAvailable(survey))
        {
            throw new InvalidOperationException("Anket şu anda aktif değil.");
        }

        if (survey.AccessType == Domain.Enums.AccessType.Internal && !_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Bu anket yalnızca dahili kullanıcılar için erişilebilir. Lütfen giriş yapın.");
        }

        var now = DateTime.Now;
        var isNewParticipant = false;
        Participant participant = null!;

        if (_currentUserService.IsAuthenticated)
        {
            var username = _currentUserService.Username;
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException("Kullanıcı adı bulunamadı.");
            }

            var existingParticipant = await _participantRepository.GetByLdapUsernameAsync(username, cancellationToken);
            if (existingParticipant is null)
            {
                participant = Participant.CreateInternal(username);
                isNewParticipant = true;
            }
            else
            {
                participant = existingParticipant;
            }
        }
        else
        {
            if (!request.ExternalId.HasValue)
            {
                throw new InvalidOperationException("Anonim katılım için dış kimlik gereklidir.");
            }

            var externalId = request.ExternalId.Value;
            var existingParticipant = await _participantRepository.GetByExternalIdAsync(externalId, cancellationToken);
            if (existingParticipant is null)
            {
                participant = Participant.CreateAnonymous(externalId);
                isNewParticipant = true;
            }
            else
            {
                participant = existingParticipant;
            }
        }

        if (isNewParticipant)
        {
            await _participantRepository.AddAsync(participant, cancellationToken);
        }

        var participantId = participant.Id;
        var existingParticipation = await _participationRepository.GetBySurveyAndParticipantAsync(survey.Id, participantId, cancellationToken);
        if (existingParticipation is not null)
        {

            if (existingParticipation.CompletedAt.HasValue)
            {
                throw new InvalidOperationException("Bu anketi zaten tamamladınız. Tekrar katılamazsınız.");
            }

            return existingParticipation.Id;
        }

        var participation = Participation.Start(survey.Id, participantId, now, request.IpAddress);

        await _participationRepository.AddAsync(participation, cancellationToken);

        return participation.Id;
    }

    private static bool IsAvailable(Survey survey)
    {
        var now = DateTime.Now;

        if (!survey.IsPublished)
        {
            return false;
        }

        if (survey.StartDate.HasValue && survey.StartDate.Value > now)
        {
            return false;
        }

        if (survey.EndDate.HasValue && survey.EndDate.Value <= now)
        {
            return false;
        }

        return true;
    }
}
