using System.Collections.Generic;
using SurveyBackend.Application.Modules.Authorization.DTOs;

namespace SurveyBackend.Application.Modules.Authorization.Queries;

public sealed record GetPermissionsQuery() : ICommand<IReadOnlyCollection<PermissionDto>>;
