using System.Linq;
using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Domain.Departments;
using SurveyBackend.Domain.Roles;
using SurveyBackend.Domain.Users;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Seeding;

public sealed class DatabaseSeeder
{
    private static readonly (string Name, string Description)[] DefaultPermissions =
    [
        ("CreateSurvey", "Anket oluşturma izni"),
        ("EditSurvey", "Anket düzenleme izni"),
        ("ViewSurveyResults", "Anket sonuçlarını görüntüleme izni"),
        ("ManageUsers", "Kullanıcı yönetim izni"),
        ("ManageDepartment", "Departman yönetim izni")
    ];

    private static readonly string[] ManagerPermissions =
    [
        "CreateSurvey",
        "EditSurvey",
        "ViewSurveyResults",
        "ManageDepartment"
    ];

    private readonly SurveyBackendDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private static readonly int SystemDepartmentId = 1;

    public DatabaseSeeder(SurveyBackendDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedSystemDepartmentAsync(cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await SeedPermissionsAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedLocalAdminAsync(cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        foreach (var permission in DefaultPermissions)
        {
            if (!await _dbContext.Permissions.AnyAsync(p => p.Name == permission.Name, cancellationToken))
            {
                _dbContext.Permissions.Add(Permission.Create(permission.Name, permission.Description));
            }
        }

    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var permissions = await _dbContext.Permissions.ToListAsync(cancellationToken);

        await EnsureRoleAsync("Admin", "Sistem yöneticisi", permissions.Select(p => p.Name).ToArray(), permissions, cancellationToken);
        await EnsureRoleAsync("Manager", "Departman yöneticisi", ManagerPermissions, permissions, cancellationToken);
    }

    private async Task EnsureRoleAsync(string roleName, string description, IReadOnlyCollection<string> permissionNames, List<Permission> allPermissions, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role is null)
        {
            role = Role.Create(roleName, description);
            _dbContext.Roles.Add(role);
        }
        else
        {
            role.UpdateDetails(roleName, description);
        }

        var desiredPermissionIds = allPermissions
            .Where(p => permissionNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .Select(p => p.Id)
            .ToHashSet();

        var existingPermissionIds = role.Permissions.Select(rp => rp.PermissionId).ToHashSet();

        foreach (var permissionId in desiredPermissionIds.Except(existingPermissionIds))
        {
            role.Permissions.Add(new RolePermission(role.Id, permissionId));
        }

        foreach (var permissionId in existingPermissionIds.Except(desiredPermissionIds).ToList())
        {
            var rolePermission = role.Permissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            if (rolePermission is not null)
            {
                role.Permissions.Remove(rolePermission);
            }
        }
    }

    private async Task SeedLocalAdminAsync(CancellationToken cancellationToken)
    {
        var adminUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "admin", cancellationToken);
        if (adminUser is not null)
        {
            return;
        }

        var hash = _passwordHasher.Hash("Admin123!");
        var user = User.CreateSuperAdmin("admin", "admin@local", SystemDepartmentId, hash);
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    private async Task SeedSystemDepartmentAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Departments.AnyAsync(d => d.Id == SystemDepartmentId, cancellationToken))
        {
            return;
        }

        var department = Department.Create("System", "system");
        await _dbContext.Departments.AddAsync(department, cancellationToken);
    }
}
