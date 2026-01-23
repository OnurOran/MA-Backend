using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Roles;

public class Permission : CommonEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private Permission()
    {
    }

    public Permission(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public static Permission Create(string name, string description)
    {
        return new Permission
        {
            Name = name,
            Description = description
        };
    }
}
