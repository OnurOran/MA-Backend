namespace SurveyBackend.Application.Modules.Auth.Models;

public sealed record JwtTokenResult(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    int AccessTokenExpiresInSeconds);
