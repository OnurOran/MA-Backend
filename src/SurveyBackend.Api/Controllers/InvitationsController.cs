using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Invitations.Commands.Cancel;
using SurveyBackend.Application.Invitations.Commands.Create;
using SurveyBackend.Application.Invitations.Commands.Import;
using SurveyBackend.Application.Invitations.Commands.Resend;
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

    [HttpPost("surveys/{surveyId:int}/resend")]
    public async Task<ActionResult<int>> Resend(int surveyId, [FromBody] SendInvitationsRequest request, CancellationToken cancellationToken)
    {
        var command = new ResendInvitationsCommand(surveyId, request.BaseUrl);
        var sentCount = await _mediator.SendAsync<ResendInvitationsCommand, int>(command, cancellationToken);
        return Ok(new { sentCount });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var command = new CancelInvitationCommand(id);
        await _mediator.SendAsync<CancelInvitationCommand, bool>(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("template")]
    public IActionResult DownloadTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Davetiyeler");

        // Headers
        worksheet.Cell(1, 1).Value = "Ad";
        worksheet.Cell(1, 2).Value = "Soyad";
        worksheet.Cell(1, 3).Value = "Email";
        worksheet.Cell(1, 4).Value = "Telefon";
        worksheet.Cell(1, 5).Value = "Gönderim";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

        // Example rows
        worksheet.Cell(2, 1).Value = "Ahmet";
        worksheet.Cell(2, 2).Value = "Yılmaz";
        worksheet.Cell(2, 3).Value = "ahmet.yilmaz@example.com";
        worksheet.Cell(2, 4).Value = "";
        worksheet.Cell(2, 5).Value = "Email";

        worksheet.Cell(3, 1).Value = "Ayşe";
        worksheet.Cell(3, 2).Value = "Demir";
        worksheet.Cell(3, 3).Value = "";
        worksheet.Cell(3, 4).Value = "+905551234567";
        worksheet.Cell(3, 5).Value = "Sms";

        worksheet.Cell(4, 1).Value = "Mehmet";
        worksheet.Cell(4, 2).Value = "Kaya";
        worksheet.Cell(4, 3).Value = "mehmet@example.com";
        worksheet.Cell(4, 4).Value = "";
        worksheet.Cell(4, 5).Value = "";

        // Style example rows
        var exampleRange = worksheet.Range(2, 1, 4, 5);
        exampleRange.Style.Font.Italic = true;
        exampleRange.Style.Font.FontColor = XLColor.Gray;

        // Add instructions sheet
        var instructionsSheet = workbook.Worksheets.Add("Talimatlar");
        instructionsSheet.Cell(1, 1).Value = "Davetiye İçe Aktarma Talimatları";
        instructionsSheet.Cell(1, 1).Style.Font.Bold = true;
        instructionsSheet.Cell(1, 1).Style.Font.FontSize = 14;

        instructionsSheet.Cell(3, 1).Value = "Zorunlu Alanlar:";
        instructionsSheet.Cell(3, 1).Style.Font.Bold = true;
        instructionsSheet.Cell(4, 1).Value = "• Ad - Davetiyenin gönderileceği kişinin adı";
        instructionsSheet.Cell(5, 1).Value = "• Soyad - Davetiyenin gönderileceği kişinin soyadı";

        instructionsSheet.Cell(7, 1).Value = "İletişim Bilgileri:";
        instructionsSheet.Cell(7, 1).Style.Font.Bold = true;
        instructionsSheet.Cell(8, 1).Value = "• Email - E-posta ile gönderim için gerekli";
        instructionsSheet.Cell(9, 1).Value = "• Telefon - SMS ile gönderim için gerekli (örn: +905551234567)";

        instructionsSheet.Cell(11, 1).Value = "Gönderim Yöntemi:";
        instructionsSheet.Cell(11, 1).Style.Font.Bold = true;
        instructionsSheet.Cell(12, 1).Value = "• 'Email' veya 'Sms' yazabilirsiniz";
        instructionsSheet.Cell(13, 1).Value = "• Boş bırakırsanız, dolu olan alana göre otomatik belirlenir";
        instructionsSheet.Cell(14, 1).Value = "• Sadece Email doluysa → Email gönderilir";
        instructionsSheet.Cell(15, 1).Value = "• Sadece Telefon doluysa → SMS gönderilir";

        instructionsSheet.Cell(17, 1).Value = "Notlar:";
        instructionsSheet.Cell(17, 1).Style.Font.Bold = true;
        instructionsSheet.Cell(18, 1).Value = "• İlk satır başlık satırıdır, silmeyin";
        instructionsSheet.Cell(19, 1).Value = "• Örnek satırları silebilir veya üzerine yazabilirsiniz";
        instructionsSheet.Cell(20, 1).Value = "• Aynı email/telefon için mükerrer kayıt eklenmez";

        // Adjust column widths
        worksheet.Column(1).Width = 15;
        worksheet.Column(2).Width = 15;
        worksheet.Column(3).Width = 30;
        worksheet.Column(4).Width = 20;
        worksheet.Column(5).Width = 12;
        instructionsSheet.Column(1).Width = 60;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "davetiye_sablonu.xlsx");
    }
}

public sealed record SendInvitationsRequest(string BaseUrl);
