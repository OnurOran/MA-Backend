namespace SurveyBackend.Infrastructure.Configurations;

public sealed class SmsSettings
{
    public bool Enabled { get; set; } = false;
    public string ApiUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
}
