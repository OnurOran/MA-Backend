using System;

namespace SurveyBackend.Application.Interfaces.Identity;

public interface IAuthorizationService
{
    Task EnsurePermissionAsync(string permissionName, CancellationToken cancellationToken);
    Task EnsureDepartmentScopeAsync(int departmentId, CancellationToken cancellationToken);
    Task EnsureRoleManagementAsync(int departmentId, CancellationToken cancellationToken);
}
