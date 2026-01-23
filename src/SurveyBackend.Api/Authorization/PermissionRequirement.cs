using Microsoft.AspNetCore.Authorization;

namespace SurveyBackend.Api.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    private readonly HashSet<string> _permissions;

    public PermissionRequirement(IEnumerable<string> permissions)
    {
        if (permissions is null)
        {
            throw new ArgumentNullException(nameof(permissions));
        }

        var normalized = permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Select(permission => permission.Trim())
            .ToArray();

        if (normalized.Length == 0)
        {
            throw new ArgumentException("At least one permission must be provided.", nameof(permissions));
        }

        _permissions = new HashSet<string>(normalized, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<string> Permissions => _permissions;

    public bool IsSatisfiedBy(IEnumerable<string>? grantedPermissions)
    {
        if (grantedPermissions is null)
        {
            return false;
        }

        return grantedPermissions.Any(permission => _permissions.Contains(permission));
    }
}
