using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SurveyBackend.Application.Interfaces.Identity;

namespace SurveyBackend.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private IReadOnlyCollection<string>? _permissions;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public int? DepartmentId
    {
        get
        {
            var departmentClaim = User?.FindFirst("departmentId")?.Value;
            return int.TryParse(departmentClaim, out var departmentId) ? departmentId : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsSuperAdmin => bool.TryParse(User?.FindFirst("isSuperAdmin")?.Value, out var value) && value;

    public string? Username => User?.FindFirst("username")?.Value;

    public bool HasPermission(string permission)
    {
        if (IsSuperAdmin)
        {
            return true;
        }

        return Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    private IReadOnlyCollection<string> Permissions
    {
        get
        {
            if (_permissions is not null)
            {
                return _permissions;
            }

            var permissionsJson = User?.FindFirst("permissions")?.Value;
            if (string.IsNullOrWhiteSpace(permissionsJson))
            {
                _permissions = Array.Empty<string>();
            }
            else
            {
                try
                {
                    _permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson) ?? new List<string>();
                }
                catch
                {
                    _permissions = Array.Empty<string>();
                }
            }

            return _permissions;
        }
    }
}
