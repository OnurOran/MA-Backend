namespace SurveyBackend.Application.Interfaces.Notifications;

public interface IEmailService
{
    Task SendInvitationEmailAsync(
        string toEmail,
        string firstName,
        string lastName,
        string surveyTitle,
        string invitationUrl,
        CancellationToken cancellationToken);
}
