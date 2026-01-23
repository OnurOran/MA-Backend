namespace SurveyBackend.Application.Modules.Authorization.Commands;

public sealed record AssignRoleToUserCommand(int UserId, int RoleId) : ICommand<bool>;
