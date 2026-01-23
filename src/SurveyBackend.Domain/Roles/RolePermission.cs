using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Roles;

public class RolePermission : CommonEntity
{
    public int RoleId { get; private set; }
    public int PermissionId { get; private set; }

    public Role Role { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;

    private RolePermission()
    {
    }

    public RolePermission(int roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}
