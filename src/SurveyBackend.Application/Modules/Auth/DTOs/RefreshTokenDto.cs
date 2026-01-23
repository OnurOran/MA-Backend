namespace SurveyBackend.Application.Modules.Auth.DTOs;

public sealed record RefreshTokenDto(string Token, DateTime ExpiresAt);
