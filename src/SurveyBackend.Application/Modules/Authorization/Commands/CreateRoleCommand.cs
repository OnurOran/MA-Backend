using System.Collections.Generic;

namespace SurveyBackend.Application.Modules.Authorization.Commands;

public sealed record CreateRoleCommand(string Name, string Description, IReadOnlyCollection<string> Permissions) : ICommand<int>;
