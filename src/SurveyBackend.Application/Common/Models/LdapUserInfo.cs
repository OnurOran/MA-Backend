namespace SurveyBackend.Application.Common.Models;

public sealed record LdapUserInfo(
    Guid ExternalId,
    string Username,
    string Email,
    string DepartmentName
);
