using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace SurveyBackend.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    private const string PermissionsClaimType = "permissions";
    private const string IndividualPermissionClaimType = "permission";
    private const string IsSuperAdminClaimType = "isSuperAdmin";

    public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        _logger.LogInformation("HandleRequirementAsync called for permissions: {Permissions}", string.Join(", ", requirement.Permissions));
        _logger.LogInformation("User authenticated: {IsAuthenticated}", context.User.Identity?.IsAuthenticated);

        var superAdminClaim = context.User.FindFirst(IsSuperAdminClaimType);
        _logger.LogInformation("SuperAdmin claim: {Claim}", superAdminClaim?.Value ?? "null");

        if (IsSuperAdmin(context))
        {
            _logger.LogInformation("User is SuperAdmin - granting access");
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        _logger.LogInformation("User is NOT SuperAdmin - checking permissions");

        var directPermissions = context.User.FindAll(IndividualPermissionClaimType).Select(p => p.Value).ToArray();
        if (directPermissions.Length > 0 && requirement.IsSatisfiedBy(directPermissions))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var permissionsClaim = context.User.FindFirst(PermissionsClaimType);
        if (permissionsClaim is null || string.IsNullOrWhiteSpace(permissionsClaim.Value))
        {
            return Task.CompletedTask;
        }

        try
        {
            var grantedPermissions = ParsePermissions(permissionsClaim.Value);
            if (requirement.IsSatisfiedBy(grantedPermissions))
            {
                context.Succeed(requirement);
            }
        }
        catch (JsonException)
        {

        }

        return Task.CompletedTask;
    }

    private static bool IsSuperAdmin(AuthorizationHandlerContext context)
    {
        var isSuperAdmin = context.User.FindFirst(IsSuperAdminClaimType)?.Value;
        return isSuperAdmin is not null && bool.TryParse(isSuperAdmin, out var result) && result;
    }

    private static IEnumerable<string> ParsePermissions(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return Array.Empty<string>();
        }

        var trimmed = rawValue.Trim();
        if (trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            var parsed = JsonSerializer.Deserialize<string[]>(trimmed);
            return parsed ?? Array.Empty<string>();
        }

        return trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(permission => permission.Trim().Trim('"'));
    }
}
