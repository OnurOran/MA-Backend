using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Departments;
using SurveyBackend.Domain.Roles;

namespace SurveyBackend.Domain.Users;

public class UserRole : CommonEntity
{
    public int UserId { get; private set; }
    public int RoleId { get; private set; }
    public int DepartmentId { get; private set; }

    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public Department? Department { get; private set; }

    private UserRole()
    {
    }

    public UserRole(int userId, int roleId, int departmentId)
    {
        UserId = userId;
        RoleId = roleId;
        DepartmentId = departmentId;
    }
}
