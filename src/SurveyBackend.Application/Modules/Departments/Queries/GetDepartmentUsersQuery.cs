using System.Collections.Generic;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Departments.DTOs;

namespace SurveyBackend.Application.Modules.Departments.Queries;

public sealed record GetDepartmentUsersQuery(int DepartmentId) : ICommand<IReadOnlyCollection<DepartmentUserDto>>, IDepartmentScopedCommand;
