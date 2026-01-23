using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Auth.Commands.Logout;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUserService _currentUserService;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUserService currentUserService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> HandleAsync(LogoutCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.UserId != _currentUserService.UserId)
        {
            return false;
        }

        refreshToken.Revoke(DateTime.Now);
        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        return true;
    }
}
