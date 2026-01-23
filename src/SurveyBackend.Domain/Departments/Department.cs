using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Departments;

public class Department : CommonEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string ExternalIdentifier { get; private set; } = null!;

    private Department()
    {
    }

    private Department(string name, string externalIdentifier)
    {
        Name = name;
        ExternalIdentifier = externalIdentifier;
    }

    public static Department Create(string name, string externalIdentifier)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Department name cannot be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(externalIdentifier))
        {
            throw new ArgumentException("Department external identifier cannot be empty.", nameof(externalIdentifier));
        }

        return new Department(name.Trim(), externalIdentifier.Trim());
    }
}
