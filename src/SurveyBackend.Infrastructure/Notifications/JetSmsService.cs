using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SurveyBackend.Application.Interfaces.Notifications;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Notifications;

public sealed class JetSmsService : ISmsService
{
    private const string ApiUrl = "https://api.jetsms.com.tr/SMS-Web/HttpSmsSend";
    private const string Username = "metroweb";
    private const string Password = "528TC1432tc__*";
    private const string TransmissionId = "METRO IST";

    private static readonly HttpClient HttpClient = new();

    private readonly SmsSettings _settings;
    private readonly ILogger<JetSmsService> _logger;

    public JetSmsService(
        IOptions<SmsSettings> settings,
        ILogger<JetSmsService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendInvitationSmsAsync(
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
            return;
        }

        try
        {
            var encodedMessage = HttpUtility.UrlEncode(message, Encoding.GetEncoding("iso-8859-9"));
            var postData = $"Password={Password}&Username={Username}&Msisdns={phoneNumber}&TransmissionID={TransmissionId}&Messages={encodedMessage}";
            var content = new StringContent(postData, Encoding.GetEncoding("iso-8859-9"), "application/x-www-form-urlencoded");

            var response = await HttpClient.PostAsync(ApiUrl, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "SMS sent successfully to {PhoneNumber}. Response: {Response}",
                    phoneNumber,
                    responseContent);
            }
            else
            {
                _logger.LogError(
                    "Failed to send SMS to {PhoneNumber}. Status: {StatusCode}, Response: {Response}",
                    phoneNumber,
                    response.StatusCode,
                    responseContent);
                throw new InvalidOperationException($"JetSMS API returned {response.StatusCode}: {responseContent}");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", phoneNumber);
            throw;
        }
    }
}
