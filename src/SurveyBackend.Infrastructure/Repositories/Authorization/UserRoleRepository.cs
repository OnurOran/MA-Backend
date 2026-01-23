using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SurveyBackend.Domain.Users;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Authorization;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public UserRoleRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AssignRoleAsync(int userId, int roleId, int departmentId, CancellationToken cancellationToken)
    {
        if (await _dbContext.UserRoles.AnyAsync(
                ur => ur.UserId == userId && ur.RoleId == roleId && ur.DepartmentId == departmentId,
                cancellationToken))
        {
            return;
        }

        _dbContext.UserRoles.Add(new UserRole(userId, roleId, departmentId));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleAsync(int userId, int roleId, int departmentId, CancellationToken cancellationToken)
    {
        var userRole = await _dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.DepartmentId == departmentId, cancellationToken);
        if (userRole is null)
        {
            return;
        }

        _dbContext.UserRoles.Remove(userRole);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAllRolesAsync(int userId, CancellationToken cancellationToken)
    {
        var roles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        if (roles.Count == 0)
        {
            return;
        }

        _dbContext.UserRoles.RemoveRange(roles);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionsForUserAsync(int userId, int departmentId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId && ur.DepartmentId == departmentId)
            .SelectMany(ur => ur.Role.Permissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(int userId, string roleName, int departmentId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.DepartmentId == departmentId && ur.Role.Name == roleName, cancellationToken);
    }
}
