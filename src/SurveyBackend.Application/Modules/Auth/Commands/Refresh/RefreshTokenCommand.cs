namespace SurveyBackend.Application.Modules.Auth.Commands.Refresh;

using SurveyBackend.Application.Modules.Auth.DTOs;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthTokensDto>;
