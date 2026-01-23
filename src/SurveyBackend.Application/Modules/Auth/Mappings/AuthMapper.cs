using SurveyBackend.Application.Modules.Auth.Commands.Login;
using SurveyBackend.Application.Modules.Auth.Commands.Refresh;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Domain.Users;

namespace SurveyBackend.Application.Modules.Auth.Mappings;

public static class AuthMapper
{
    public static LoginCommand ToLoginCommand(LoginRequest request) =>
        new(request.Username, request.Password);

    public static RefreshTokenCommand ToRefreshTokenCommand(RefreshTokenRequest request) =>
        new(request.RefreshToken ?? string.Empty);

    public static AuthTokensDto ToAuthTokensDto(TokenResult result) =>
        new(result.AccessToken, result.RefreshToken, result.ExpiresIn);

    public static RefreshTokenDto ToRefreshTokenDto(RefreshToken refreshToken) =>
        new(refreshToken.Token, refreshToken.ExpiresAt);
}
