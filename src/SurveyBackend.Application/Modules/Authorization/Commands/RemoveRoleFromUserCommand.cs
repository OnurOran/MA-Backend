namespace SurveyBackend.Application.Modules.Authorization.Commands;

public sealed record RemoveRoleFromUserCommand(int UserId, int RoleId) : ICommand<bool>;
