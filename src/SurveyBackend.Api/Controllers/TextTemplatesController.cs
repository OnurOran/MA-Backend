using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.TextTemplates.Commands.Create;
using SurveyBackend.Application.TextTemplates.Commands.Delete;
using SurveyBackend.Application.TextTemplates.Commands.Update;
using SurveyBackend.Application.TextTemplates.DTOs;
using SurveyBackend.Application.TextTemplates.Queries.GetTextTemplates;
using SurveyBackend.Application.TextTemplates.Queries.GetTextTemplatesByType;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PermissionPolicies.ManageUsersOrDepartment)]
public class TextTemplatesController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public TextTemplatesController(IAppMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("department")]
    public async Task<ActionResult<IReadOnlyCollection<TextTemplateDto>>> GetByDepartment(CancellationToken cancellationToken)
    {
        var departmentId = _currentUserService.DepartmentId;
        if (!departmentId.HasValue)
        {
            return Forbid();
        }

        var query = new GetTextTemplatesQuery(departmentId.Value);
        var response = await _mediator.SendAsync<GetTextTemplatesQuery, IReadOnlyCollection<TextTemplateDto>>(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("department/by-type")]
    public async Task<ActionResult<IReadOnlyCollection<TextTemplateDto>>> GetByDepartmentAndType(
        [FromQuery] TextTemplateType type,
        CancellationToken cancellationToken)
    {
        var departmentId = _currentUserService.DepartmentId;
        if (!departmentId.HasValue)
        {
            return Forbid();
        }

        var query = new GetTextTemplatesByTypeQuery(departmentId.Value, type);
        var response = await _mediator.SendAsync<GetTextTemplatesByTypeQuery, IReadOnlyCollection<TextTemplateDto>>(query, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTextTemplateCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.SendAsync<CreateTextTemplateCommand, int>(command, cancellationToken);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTextTemplateCommand command, CancellationToken cancellationToken)
    {
        var fullCommand = command with { Id = id };
        await _mediator.SendAsync<UpdateTextTemplateCommand, bool>(fullCommand, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteTextTemplateCommand(id);
        await _mediator.SendAsync<DeleteTextTemplateCommand, bool>(command, cancellationToken);
        return NoContent();
    }
}
