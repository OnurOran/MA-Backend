using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentRoleController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly IValidator<AssignRoleToUserCommand> _assignValidator;
    private readonly IValidator<RemoveRoleFromUserCommand> _removeValidator;

    public DepartmentRoleController(
        IAppMediator mediator,
        IValidator<AssignRoleToUserCommand> assignValidator,
        IValidator<RemoveRoleFromUserCommand> removeValidator)
    {
        _mediator = mediator;
        _assignValidator = assignValidator;
        _removeValidator = removeValidator;
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _assignValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        await _mediator.SendAsync<AssignRoleToUserCommand, bool>(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _removeValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        await _mediator.SendAsync<RemoveRoleFromUserCommand, bool>(command, cancellationToken);
        return NoContent();
    }
}
