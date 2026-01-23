using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Files;
using SurveyBackend.Application.Surveys.Queries.GetAttachment;
using SurveyBackend.Application.Participations.Queries.GetAnswerAttachment;

namespace SurveyBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly IAppMediator _mediator;
    private readonly IFileStorage _fileStorage;

    public AttachmentsController(IAppMediator mediator, IFileStorage fileStorage)
    {
        _mediator = mediator;
        _fileStorage = fileStorage;
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Download(int id, [FromQuery] bool download = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.SendAsync<GetAttachmentQuery, AttachmentDownloadResult>(new GetAttachmentQuery(id), cancellationToken);
            var stream = await _fileStorage.OpenReadAsync(result.StoragePath, cancellationToken);

            if (download)
            {
                return File(stream, result.ContentType, result.FileName);
            }
            else
            {

                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{result.FileName}\"");
                return File(stream, result.ContentType);
            }
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [AllowAnonymous]
    [HttpGet("answers/{id:int}")]
    public async Task<IActionResult> DownloadAnswerAttachment(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.SendAsync<GetAnswerAttachmentQuery, AttachmentDownloadResult>(new GetAnswerAttachmentQuery(id), cancellationToken);
            var stream = await _fileStorage.OpenReadAsync(result.StoragePath, cancellationToken);
            return File(stream, result.ContentType, result.FileName);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}
