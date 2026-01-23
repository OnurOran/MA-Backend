using SurveyBackend.Domain.Users;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<int> DeleteExpiredTokensAsync(DateTime cutoffDate, CancellationToken cancellationToken);
}
