using System.Collections.Generic;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IUserRoleRepository
{
    Task AssignRoleAsync(int userId, int roleId, int departmentId, CancellationToken cancellationToken);
    Task RemoveRoleAsync(int userId, int roleId, int departmentId, CancellationToken cancellationToken);
    Task RemoveAllRolesAsync(int userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetPermissionsForUserAsync(int userId, int departmentId, CancellationToken cancellationToken);
    Task<bool> UserHasRoleAsync(int userId, string roleName, int departmentId, CancellationToken cancellationToken);
}
