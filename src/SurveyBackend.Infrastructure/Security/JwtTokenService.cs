using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Domain.Users;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task<JwtTokenResult> GenerateTokensAsync(User user, IEnumerable<string> permissions, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var expiresAt = now.AddMinutes(_settings.AccessTokenMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var permissionsJson = JsonSerializer.Serialize(permissions.Distinct(StringComparer.OrdinalIgnoreCase));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("username", user.Username),
            new("departmentId", user.DepartmentId.ToString()),
            new("permissions", permissionsJson),
            new("isSuperAdmin", user.IsSuperAdmin.ToString().ToLowerInvariant())
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = signingCredentials
        };

        var accessToken = _tokenHandler.CreateEncodedJwt(tokenDescriptor);
        var refreshToken = GenerateSecureToken();
        var refreshExpiresAt = now.AddDays(_settings.RefreshTokenDays);
        var expiresInSeconds = (int)TimeSpan.FromMinutes(_settings.AccessTokenMinutes).TotalSeconds;

        var result = new JwtTokenResult(accessToken, expiresAt, refreshToken, refreshExpiresAt, expiresInSeconds);
        return Task.FromResult(result);
    }

    private static string GenerateSecureToken()
    {
        Span<byte> buffer = stackalloc byte[64];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }
}
