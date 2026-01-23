using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Queries.GetDepartmentDashboard;
using SurveyBackend.Application.Surveys.Queries.GetGlobalDashboard;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public DashboardController(IAppMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet("department/{departmentId:int}")]
    public async Task<ActionResult<DepartmentDashboardDto>> GetDepartmentStats(int departmentId, CancellationToken cancellationToken)
    {
        var query = new GetDepartmentDashboardQuery(departmentId);
        var response = await _mediator.SendAsync<GetDepartmentDashboardQuery, DepartmentDashboardDto>(query, cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet("department")]
    public async Task<ActionResult<DepartmentDashboardDto>> GetMyDepartmentStats(CancellationToken cancellationToken)
    {
        var departmentId = _currentUserService.DepartmentId;
        if (!departmentId.HasValue)
        {
            return Forbid();
        }

        var query = new GetDepartmentDashboardQuery(departmentId.Value);
        var response = await _mediator.SendAsync<GetDepartmentDashboardQuery, DepartmentDashboardDto>(query, cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsers)]
    [HttpGet("global")]
    public async Task<ActionResult<GlobalDashboardDto>> GetGlobalStats(CancellationToken cancellationToken)
    {
        var response = await _mediator.SendAsync<GetGlobalDashboardQuery, GlobalDashboardDto>(new GetGlobalDashboardQuery(), cancellationToken);
        return Ok(response);
    }
}
