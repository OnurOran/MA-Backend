using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Notifications;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Invitations.Commands.Send;

public sealed class SendInvitationsCommandHandler : ICommandHandler<SendInvitationsCommand, int>
{
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public SendInvitationsCommandHandler(
        ISurveyInvitationRepository invitationRepository,
        ISurveyRepository surveyRepository,
        IEmailService emailService,
        ISmsService smsService,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _invitationRepository = invitationRepository;
        _surveyRepository = surveyRepository;
        _emailService = emailService;
        _smsService = smsService;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<int> HandleAsync(SendInvitationsCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı oturumu bulunamadı.");
        }

        var survey = await _surveyRepository.GetByIdAsync(command.SurveyId, cancellationToken)
            ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

        var pendingInvitations = await _invitationRepository.GetPendingBySurveyIdAsync(command.SurveyId, cancellationToken);

        if (pendingInvitations.Count == 0)
        {
            return 0;
        }

        var baseUrl = command.BaseUrl.TrimEnd('/');
        var sentCount = 0;

        foreach (var invitation in pendingInvitations)
        {
            var invitationUrl = $"{baseUrl}/s/{invitation.Token}";

            try
            {
                if (invitation.DeliveryMethod == DeliveryMethod.Email)
                {
                    await _emailService.SendInvitationEmailAsync(
                        invitation.Email!,
                        invitation.FirstName,
                        invitation.LastName,
                        survey.Title,
                        invitationUrl,
                        cancellationToken);
                }
                else
                {
                    await _smsService.SendInvitationSmsAsync(
                        invitation.Phone!,
                        invitation.FirstName,
                        survey.Title,
                        invitationUrl,
                        cancellationToken);
                }

                invitation.MarkAsSent();
                await _invitationRepository.UpdateAsync(invitation, cancellationToken);
                sentCount++;
            }
            catch
            {
                // Continue with other invitations even if one fails
                // The failed invitation will remain in Pending status
            }
        }

        return sentCount;
    }
}
