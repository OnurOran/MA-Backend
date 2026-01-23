using System.Collections.Generic;
using SurveyBackend.Application.Modules.Departments.DTOs;

namespace SurveyBackend.Application.Modules.Departments.Queries;

public sealed record GetDepartmentsQuery() : ICommand<IReadOnlyCollection<DepartmentDto>>;
