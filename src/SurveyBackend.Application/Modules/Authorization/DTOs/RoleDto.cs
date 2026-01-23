namespace SurveyBackend.Application.Modules.Authorization.DTOs;

public sealed record RoleDto(int Id, string Name, string Description, IReadOnlyCollection<string> Permissions);
