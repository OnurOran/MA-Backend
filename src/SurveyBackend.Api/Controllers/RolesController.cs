using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Authorization.Commands;
using SurveyBackend.Application.Modules.Authorization.DTOs;
using SurveyBackend.Application.Modules.Authorization.Queries;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly IValidator<CreateRoleCommand> _createRoleValidator;

    public RolesController(IAppMediator mediator, IValidator<CreateRoleCommand> createRoleValidator)
    {
        _mediator = mediator;
        _createRoleValidator = createRoleValidator;
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.SendAsync<GetRolesQuery, IReadOnlyCollection<RoleDto>>(new GetRolesQuery(), cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = PermissionPolicies.ManageUsers)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _createRoleValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var roleId = await _mediator.SendAsync<CreateRoleCommand, int>(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = roleId }, null);
    }
}
