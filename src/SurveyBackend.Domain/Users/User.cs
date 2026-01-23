using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Users;

public class User : CommonEntity
{
    public int Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public int DepartmentId { get; private set; }
    public string? PasswordHash { get; private set; }
    public bool IsSuperAdmin { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<UserRole> Roles { get; private set; } = new List<UserRole>();

    private User()
    {
    }

    private User(string username, string email, int departmentId, bool isSuperAdmin, string? passwordHash)
    {
        Username = username;
        Email = email;
        DepartmentId = departmentId;
        IsSuperAdmin = isSuperAdmin;
        PasswordHash = passwordHash;
    }

    public static User Create(string username, string email, int departmentId)
    {
        return new User(username, email, departmentId, false, null);
    }

    public static User CreateSuperAdmin(string username, string email, int departmentId, string passwordHash)
    {
        return new User(username, email, departmentId, true, passwordHash);
    }

    public void UpdateContactInformation(string email, int departmentId)
    {
        Email = email;
        DepartmentId = departmentId;
    }

    public RefreshToken IssueRefreshToken(string token, DateTime expiresAt)
    {
        var refreshToken = RefreshToken.Create(Id, token, expiresAt);
        RefreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void UpdateSuperAdminPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        IsSuperAdmin = true;
    }
}
