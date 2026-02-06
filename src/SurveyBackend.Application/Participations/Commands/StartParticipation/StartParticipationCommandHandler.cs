using SurveyBackend.Application.Common;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Participations.Commands.StartParticipation;

public sealed class StartParticipationCommandHandler : ICommandHandler<StartParticipationCommand, int>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ICurrentUserService _currentUserService;

    public StartParticipationCommandHandler(
        ISurveyRepository surveyRepository,
        IParticipantRepository participantRepository,
        IParticipationRepository participationRepository,
        ISurveyInvitationRepository invitationRepository,
        ICurrentUserService currentUserService)
    {
        _surveyRepository = surveyRepository;
        _participantRepository = participantRepository;
        _participationRepository = participationRepository;
        _invitationRepository = invitationRepository;
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

        // Handle invitation-only surveys
        if (survey.AccessType == AccessType.InvitationOnly)
        {
            return await HandleInvitationParticipationAsync(request, survey, cancellationToken);
        }

        if (survey.AccessType == AccessType.Internal && !_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Bu anket yalnızca dahili kullanıcılar için erişilebilir. Lütfen giriş yapın.");
        }

        var now = TimeHelper.NowInTurkey;
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

    private async Task<int> HandleInvitationParticipationAsync(
        StartParticipationCommand request,
        Survey survey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.InvitationToken))
        {
            throw new InvalidOperationException("Bu anket davetiye bazlıdır. Geçerli bir davetiye kodu gereklidir.");
        }

        var invitation = await _invitationRepository.GetByTokenAsync(request.InvitationToken, cancellationToken)
            ?? throw new InvalidOperationException("Geçersiz davetiye kodu.");

        if (invitation.SurveyId != survey.Id)
        {
            throw new InvalidOperationException("Davetiye kodu bu anket için geçerli değil.");
        }

        if (invitation.Status == InvitationStatus.Cancelled)
        {
            throw new InvalidOperationException("Bu davetiye iptal edilmiştir.");
        }

        if (invitation.Status == InvitationStatus.Completed)
        {
            throw new InvalidOperationException("Bu davetiye ile zaten katılım sağlanmıştır.");
        }

        // Check for existing participation
        if (invitation.ParticipationId.HasValue)
        {
            var existingParticipation = await _participationRepository.GetByIdAsync(
                invitation.ParticipationId.Value,
                cancellationToken);

            if (existingParticipation != null)
            {
                if (existingParticipation.CompletedAt.HasValue)
                {
                    throw new InvalidOperationException("Bu anketi zaten tamamladınız. Tekrar katılamazsınız.");
                }

                return existingParticipation.Id;
            }
        }

        var now = TimeHelper.NowInTurkey;

        // Create participant from invitation
        var participant = Participant.CreateFromInvitation(invitation.Id);
        await _participantRepository.AddAsync(participant, cancellationToken);

        // Create participation
        var participation = Participation.Start(survey.Id, participant.Id, now, request.IpAddress);
        await _participationRepository.AddAsync(participation, cancellationToken);

        // Link participation to invitation and mark as viewed
        invitation.LinkParticipation(participation.Id);
        invitation.MarkAsViewed();
        await _invitationRepository.UpdateAsync(invitation, cancellationToken);

        return participation.Id;
    }

    private static bool IsAvailable(Survey survey)
    {
        var now = TimeHelper.NowInTurkey;

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
