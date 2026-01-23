using SurveyBackend.Application.Modules.Auth.Models;
using SurveyBackend.Domain.Users;

namespace SurveyBackend.Application.Interfaces.Security;

public interface IJwtTokenService
{
    Task<JwtTokenResult> GenerateTokensAsync(User user, IEnumerable<string> permissions, CancellationToken cancellationToken);
}
