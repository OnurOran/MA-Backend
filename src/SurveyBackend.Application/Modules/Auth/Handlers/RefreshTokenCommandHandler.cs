using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Commands.Refresh;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Mappings;
using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Domain.Users;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IPermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<AuthTokensDto> HandleAsync(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingToken is null || !existingToken.IsValid)
        {
            throw new UnauthorizedAccessException("Geçersiz refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken)
                   ?? throw new InvalidOperationException("Kullanıcı bulunamadı.");

        var now = DateTime.Now;
        existingToken.Revoke(now);
        await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);

        var permissions = await ResolvePermissionsAsync(user, cancellationToken);
        var tokenResult = await _jwtTokenService.GenerateTokensAsync(user, permissions, cancellationToken);
        var newRefreshToken = user.IssueRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        var refreshTokenDto = AuthMapper.ToRefreshTokenDto(newRefreshToken);
        return AuthMapper.ToAuthTokensDto(new TokenResult(tokenResult.AccessToken, refreshTokenDto.Token, tokenResult.AccessTokenExpiresInSeconds));
    }

    private async Task<IReadOnlyCollection<string>> ResolvePermissionsAsync(User user, CancellationToken cancellationToken)
    {
        if (user.IsSuperAdmin)
        {

            var allPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
            return allPermissions.Select(p => p.Name).ToList();
        }

        return await _userRoleRepository.GetPermissionsForUserAsync(user.Id, user.DepartmentId, cancellationToken);
    }
}
