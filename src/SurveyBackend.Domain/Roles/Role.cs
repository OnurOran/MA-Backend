using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Roles;

public class Role : CommonEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public ICollection<RolePermission> Permissions { get; private set; } = new List<RolePermission>();

    private Role()
    {
    }

    public Role(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public static Role Create(string name, string description)
    {
        return new Role
        {
            Name = name,
            Description = description
        };
    }

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
