namespace SurveyBackend.Application.Interfaces.Notifications;

public interface ISmsService
{
    Task SendInvitationSmsAsync(
        string phoneNumber,
        string firstName,
        string surveyTitle,
        string invitationUrl,
        CancellationToken cancellationToken);
}
