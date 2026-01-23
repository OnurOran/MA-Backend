using SurveyBackend.Application.Interfaces.External;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Commands.Login;
using SurveyBackend.Application.Modules.Auth.DTOs;
using SurveyBackend.Application.Modules.Auth.Mappings;
using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Domain.Departments;
using SurveyBackend.Domain.Users;
using System.Text.RegularExpressions;

namespace SurveyBackend.Application.Modules.Auth.Handlers;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, AuthTokensDto>
{
    private readonly ILdapService _ldapService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public LoginCommandHandler(
        ILdapService ldapService,
        IDepartmentRepository departmentRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        IPermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository)
    {
        _ldapService = ldapService;
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<AuthTokensDto> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var localUser = await _userRepository.GetLocalByUsernameAsync(request.Username, cancellationToken);
        User user;
        Func<CancellationToken, Task>? persistUserOperation = null;
        var shouldClearRoles = false;

        if (localUser is not null && localUser.IsSuperAdmin)
        {
            if (string.IsNullOrWhiteSpace(localUser.PasswordHash) || !_passwordHasher.Verify(localUser.PasswordHash, request.Password))
            {
                throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
            }

            user = localUser;
        }
        else
        {
            var ldapUser = await _ldapService.AuthenticateAsync(request.Username, request.Password, cancellationToken);
            if (ldapUser is null)
            {
                throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
            }

            var (departmentName, externalIdentifier) = NormalizeDepartment(ldapUser.DepartmentName);

            var department = await _departmentRepository.GetByExternalIdentifierAsync(externalIdentifier, cancellationToken);
            if (department is null)
            {
                department = Department.Create(departmentName, externalIdentifier);
                await _departmentRepository.AddAsync(department, cancellationToken);
            }

            var existingUser = await _userRepository.GetByUsernameAsync(ldapUser.Username, cancellationToken);

            if (existingUser is null)
            {
                user = User.Create(ldapUser.Username, ldapUser.Email, department.Id);
                persistUserOperation = ct => _userRepository.AddAsync(user, ct);
            }
            else
            {
                if (existingUser.DepartmentId != department.Id)
                {
                    shouldClearRoles = true;
                }

                existingUser.UpdateContactInformation(ldapUser.Email, department.Id);
                user = existingUser;
                persistUserOperation = ct => _userRepository.UpdateAsync(user, ct);
            }
        }

        if (shouldClearRoles)
        {
            await _userRoleRepository.RemoveAllRolesAsync(user.Id, cancellationToken);
        }

        var permissions = await ResolvePermissionsAsync(user, cancellationToken);
        var tokenResult = await _jwtTokenService.GenerateTokensAsync(user, permissions, cancellationToken);
        var refreshToken = user.IssueRefreshToken(tokenResult.RefreshToken, tokenResult.RefreshTokenExpiresAt);

        if (persistUserOperation is not null)
        {

            await persistUserOperation(cancellationToken);
        }
        else
        {

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        var refreshTokenDto = AuthMapper.ToRefreshTokenDto(refreshToken);
        var authTokens = AuthMapper.ToAuthTokensDto(new TokenResult(tokenResult.AccessToken, refreshTokenDto.Token, tokenResult.AccessTokenExpiresInSeconds));
        return authTokens;
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

    private static (string DepartmentName, string ExternalIdentifier) NormalizeDepartment(string rawDepartment)
    {
        if (string.IsNullOrWhiteSpace(rawDepartment))
        {
            throw new UnauthorizedAccessException("Kullanıcı için geçerli bir departman bulunamadı.");
        }

        var trimmed = rawDepartment.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new UnauthorizedAccessException("Kullanıcı için geçerli bir departman bulunamadı.");
        }

        var normalized = Regex.Replace(trimmed, @"\s+", " ");
        var slug = normalized
            .ToLowerInvariant()
            .Replace(' ', '-');

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new UnauthorizedAccessException("Departman bilgisi boş olamaz.");
        }

        return (normalized, slug);
    }
}
