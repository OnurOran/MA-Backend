namespace SurveyBackend.Application.Modules.Departments.DTOs;

public sealed record DepartmentUserDto(int Id, string Username, string Email, IReadOnlyCollection<string> Roles);
