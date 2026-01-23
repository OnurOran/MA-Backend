using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Departments.DTOs;
using SurveyBackend.Application.Modules.Departments.Queries;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IAppMediator _mediator;

    public DepartmentsController(IAppMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = PermissionPolicies.ManageUsers)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.SendAsync<GetDepartmentsQuery, IReadOnlyCollection<DepartmentDto>>(new GetDepartmentsQuery(), cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet("{departmentId:int}/users")]
    public async Task<ActionResult<IReadOnlyCollection<DepartmentUserDto>>> GetUsers(int departmentId, CancellationToken cancellationToken)
    {
        var query = new GetDepartmentUsersQuery(departmentId);
        var response = await _mediator.SendAsync<GetDepartmentUsersQuery, IReadOnlyCollection<DepartmentUserDto>>(query, cancellationToken);
        return Ok(response);
    }
}
