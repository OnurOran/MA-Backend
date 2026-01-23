namespace SurveyBackend.Application.Modules.Auth.Models;

public sealed record TokenResult(string AccessToken, string RefreshToken, int ExpiresIn);
