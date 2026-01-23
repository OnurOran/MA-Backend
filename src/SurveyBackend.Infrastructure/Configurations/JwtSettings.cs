namespace SurveyBackend.Infrastructure.Configurations;

public sealed class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;

    public string AccessTokenCookieName { get; set; } = "access_token";
    public string RefreshTokenCookieName { get; set; } = "refresh_token";
    public string AccessTokenCookiePath { get; set; } = "/";
    public string RefreshTokenCookiePath { get; set; } = "/";
    public bool UseCookies { get; set; } = true;
    public bool SecureCookies { get; set; } = true;
    public string CookieSameSite { get; set; } = "None";
}
