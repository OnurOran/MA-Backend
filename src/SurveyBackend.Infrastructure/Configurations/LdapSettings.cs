namespace SurveyBackend.Infrastructure.Configurations;

public sealed class LdapSettings
{
    public string Path { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string SearchBase { get; set; } = string.Empty;
    public string UsernameAttribute { get; set; } = "sAMAccountName";
    public string EmailAttribute { get; set; } = "mail";
    public string DepartmentAttribute { get; set; } = "department";
}
