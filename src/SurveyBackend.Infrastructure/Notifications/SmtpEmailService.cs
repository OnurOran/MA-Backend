using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SurveyBackend.Application.Interfaces.Notifications;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Notifications;

public sealed class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<SmtpSettings> settings,
        ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendInvitationEmailAsync(
        string toEmail,
        string firstName,
        string lastName,
        string surveyTitle,
        string invitationUrl,
        CancellationToken cancellationToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation(
                "[DEV MODE] Email disabled. Would send invitation email to {Email} for survey '{SurveyTitle}' with URL: {Url}",
                toEmail,
                surveyTitle,
                invitationUrl);
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            _logger.LogWarning(
                "SMTP host not configured. Cannot send invitation email to {Email} for survey '{SurveyTitle}'",
                toEmail,
                surveyTitle);
            return;
        }

        var subject = $"Anket Davetiyesi: {surveyTitle}";
        var body = BuildEmailBody(firstName, lastName, surveyTitle, invitationUrl);

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Invitation email sent to {Email} for survey '{SurveyTitle}'", toEmail, surveyTitle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send invitation email to {Email} for survey '{SurveyTitle}'", toEmail, surveyTitle);
            throw;
        }
    }

    private static string BuildEmailBody(string firstName, string lastName, string surveyTitle, string invitationUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
</head>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
        <h2 style=""color: #2c3e50;"">Anket Davetiyesi</h2>
        <p>Sayın {firstName} {lastName},</p>
        <p>""{surveyTitle}"" anketine katılmanız için davet edildiniz.</p>
        <p style=""margin: 30px 0;"">
            <a href=""{invitationUrl}""
               style=""background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px;"">
                Ankete Katıl
            </a>
        </p>
        <p style=""color: #7f8c8d; font-size: 12px;"">
            Bu bağlantı size özeldir, lütfen başkalarıyla paylaşmayın.
        </p>
        <hr style=""border: none; border-top: 1px solid #eee; margin: 20px 0;"">
        <p style=""color: #7f8c8d; font-size: 12px;"">
            Metro Anket Sistemi
        </p>
    </div>
</body>
</html>";
    }
}
