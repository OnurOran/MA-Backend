using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Invitations.Commands.Create;

public sealed class CreateInvitationCommandHandler : ICommandHandler<CreateInvitationCommand, int>
{
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public CreateInvitationCommandHandler(
        ISurveyInvitationRepository invitationRepository,
        ISurveyRepository surveyRepository,
        ITokenGenerator tokenGenerator,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _invitationRepository = invitationRepository;
        _surveyRepository = surveyRepository;
        _tokenGenerator = tokenGenerator;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<int> HandleAsync(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı oturumu bulunamadı.");
        }

        var survey = await _surveyRepository.GetByIdAsync(command.SurveyId, cancellationToken)
            ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

        if (survey.AccessType != AccessType.InvitationOnly)
        {
            throw new InvalidOperationException("Bu anket davetiye bazlı değil.");
        }

        var token = await GenerateUniqueTokenAsync(cancellationToken);

        SurveyInvitation invitation;
        if (command.DeliveryMethod == DeliveryMethod.Email)
        {
            if (string.IsNullOrWhiteSpace(command.Email))
            {
                throw new InvalidOperationException("Email gönderim yöntemi için email adresi zorunludur.");
            }

            if (await _invitationRepository.EmailExistsForSurveyAsync(command.SurveyId, command.Email.Trim(), cancellationToken))
            {
                throw new InvalidOperationException($"Bu email adresi için zaten bir davetiye mevcut: {command.Email}");
            }

            invitation = SurveyInvitation.CreateForEmail(
                command.SurveyId,
                token,
                command.FirstName,
                command.LastName,
                command.Email);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(command.Phone))
            {
                throw new InvalidOperationException("SMS gönderim yöntemi için telefon numarası zorunludur.");
            }

            if (await _invitationRepository.PhoneExistsForSurveyAsync(command.SurveyId, command.Phone.Trim(), cancellationToken))
            {
                throw new InvalidOperationException($"Bu telefon numarası için zaten bir davetiye mevcut: {command.Phone}");
            }

            invitation = SurveyInvitation.CreateForSms(
                command.SurveyId,
                token,
                command.FirstName,
                command.LastName,
                command.Phone);
        }

        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return invitation.Id;
    }

    private async Task<string> GenerateUniqueTokenAsync(CancellationToken cancellationToken)
    {
        const int maxAttempts = 10;
        for (var i = 0; i < maxAttempts; i++)
        {
            var token = _tokenGenerator.GenerateInvitationToken();
            if (!await _invitationRepository.TokenExistsAsync(token, cancellationToken))
            {
                return token;
            }
        }

        throw new InvalidOperationException("Benzersiz token oluşturulamadı. Lütfen tekrar deneyin.");
    }
}
