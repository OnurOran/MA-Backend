namespace SurveyBackend.Application.Modules.Auth.DTOs;

public sealed record AuthTokensDto(string AccessToken, string RefreshToken, int ExpiresIn);
