namespace SurveyBackend.Application.Modules.Auth.Commands.Admin;

public sealed record UpdateLocalAdminPasswordCommand(string NewPassword) : ICommand<bool>;
