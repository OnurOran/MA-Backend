using System;

namespace SurveyBackend.Application.Interfaces.Identity;

public interface ICurrentUserService
{
    int? UserId { get; }
    int? DepartmentId { get; }
    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
    string? Username { get; }
    bool HasPermission(string permission);
}
