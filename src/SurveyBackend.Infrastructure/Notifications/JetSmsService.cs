using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SurveyBackend.Application.Interfaces.Notifications;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Notifications;

public sealed class JetSmsService : ISmsService
{
    private readonly SmsSettings _settings;
    private readonly ILogger<JetSmsService> _logger;

    public JetSmsService(
        IOptions<SmsSettings> settings,
        ILogger<JetSmsService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task SendInvitationSmsAsync(
        string phoneNumber,
        string firstName,
        string surveyTitle,
        string invitationUrl,
        CancellationToken cancellationToken)
    {
        var message = $"Sayin {firstName}, '{surveyTitle}' anketine katilmak icin: {invitationUrl}";

        if (!_settings.Enabled)
        {
            _logger.LogInformation(
                "[DEV MODE] SMS disabled. Would send SMS to {PhoneNumber}: {Message}",
                phoneNumber,
                message);
            return Task.CompletedTask;
        }

        if (string.IsNullOrWhiteSpace(_settings.ApiUrl) || string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            _logger.LogWarning(
                "JetSMS not configured. Cannot send SMS to {PhoneNumber}",
                phoneNumber);
            return Task.CompletedTask;
        }

        // TODO: Implement actual JetSMS API integration when credentials are provided
        _logger.LogWarning(
            "JetSMS API integration not implemented. Would send SMS to {PhoneNumber}: {Message}",
            phoneNumber,
            message);

        return Task.CompletedTask;
    }
}
