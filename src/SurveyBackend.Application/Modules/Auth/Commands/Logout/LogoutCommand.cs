namespace SurveyBackend.Application.Modules.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<bool>;
