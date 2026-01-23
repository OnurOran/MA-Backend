namespace SurveyBackend.Application.Modules.Auth.Commands.Login;

using SurveyBackend.Application.Modules.Auth.DTOs;

public sealed record LoginCommand(string Username, string Password) : ICommand<AuthTokensDto>;
