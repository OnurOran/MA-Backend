using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Invitations.DTOs;
using SurveyBackend.Application.Invitations.Queries.GetSurveyByToken;
using SurveyBackend.Application.Participations.Commands.StartParticipation;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/s")]
[AllowAnonymous]
public class TokenSurveyController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly ISurveyInvitationRepository _invitationRepository;

    public TokenSurveyController(IAppMediator mediator, ISurveyInvitationRepository invitationRepository)
    {
        _mediator = mediator;
        _invitationRepository = invitationRepository;
    }

    [HttpGet("{token}")]
    public async Task<ActionResult<TokenSurveyDto>> GetByToken(string token, CancellationToken cancellationToken)
    {
        var query = new GetSurveyByTokenQuery(token);
        var result = await _mediator.SendAsync<GetSurveyByTokenQuery, TokenSurveyDto>(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{token}/start")]
    public async Task<ActionResult<int>> Start(string token, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByTokenAsync(token, cancellationToken);
        if (invitation is null)
        {
            return NotFound("Geçersiz davetiye kodu.");
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new StartParticipationCommand(invitation.SurveyId, null)
        {
            IpAddress = ipAddress,
            InvitationToken = token
        };

        var participationId = await _mediator.SendAsync<StartParticipationCommand, int>(command, cancellationToken);
        return Ok(participationId);
    }
}
