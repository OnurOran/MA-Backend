using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Invitations.Commands.Cancel;
using SurveyBackend.Application.Invitations.Commands.Create;
using SurveyBackend.Application.Invitations.Commands.Import;
using SurveyBackend.Application.Invitations.Commands.Send;
using SurveyBackend.Application.Invitations.DTOs;
using SurveyBackend.Application.Invitations.Queries.GetSurveyInvitations;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvitationsController : ControllerBase
{
    private readonly IAppMediator _mediator;

    public InvitationsController(IAppMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("surveys/{surveyId:int}")]
    public async Task<ActionResult<IReadOnlyList<InvitationDto>>> GetBySurvey(int surveyId, CancellationToken cancellationToken)
    {
        var query = new GetSurveyInvitationsQuery(surveyId);
        var result = await _mediator.SendAsync<GetSurveyInvitationsQuery, IReadOnlyList<InvitationDto>>(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.SendAsync<CreateInvitationCommand, int>(command, cancellationToken);
        return CreatedAtAction(nameof(GetBySurvey), new { surveyId = command.SurveyId }, id);
    }

    [HttpPost("surveys/{surveyId:int}/import")]
    public async Task<ActionResult<int>> Import(int surveyId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Dosya seçilmedi.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            return BadRequest("Yalnızca Excel dosyaları (.xlsx, .xls) desteklenmektedir.");
        }

        using var stream = file.OpenReadStream();
        var command = new ImportInvitationsCommand(surveyId, stream);
        var count = await _mediator.SendAsync<ImportInvitationsCommand, int>(command, cancellationToken);
        return Ok(new { importedCount = count });
    }

    [HttpPost("surveys/{surveyId:int}/send")]
    public async Task<ActionResult<int>> Send(int surveyId, [FromBody] SendInvitationsRequest request, CancellationToken cancellationToken)
    {
        var command = new SendInvitationsCommand(surveyId, request.BaseUrl);
        var sentCount = await _mediator.SendAsync<SendInvitationsCommand, int>(command, cancellationToken);
        return Ok(new { sentCount });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var command = new CancelInvitationCommand(id);
        await _mediator.SendAsync<CancelInvitationCommand, bool>(command, cancellationToken);
        return NoContent();
    }
}

public sealed record SendInvitationsRequest(string BaseUrl);
