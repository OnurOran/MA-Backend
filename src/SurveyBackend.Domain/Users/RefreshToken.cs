using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Users;

public class RefreshToken : CommonEntity
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public User? User { get; private set; }

    public bool IsExpired => DateTime.Now >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsValid => !IsExpired && !IsRevoked;

    private RefreshToken()
    {
    }

    private RefreshToken(int userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(int userId, string token, DateTime expiresAt)
    {
        return new RefreshToken(userId, token, expiresAt);
    }

    public void Revoke(DateTime revokedAt)
    {
        RevokedAt = revokedAt;
    }
}
