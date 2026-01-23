namespace SurveyBackend.Api.Authorization;

public static class PermissionPolicies
{
    public const string ManageUsers = "ManageUsers";
    public const string ManageDepartment = "ManageDepartment";
    public const string ManageUsersOrDepartment = $"{ManageUsers}|{ManageDepartment}";
}
