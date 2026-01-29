using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Interfaces.Import;

public interface IExcelImportService
{
    Task<IReadOnlyList<InvitationImportRow>> ParseInvitationsAsync(
        Stream excelStream,
        CancellationToken cancellationToken);
}

public sealed record InvitationImportRow(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DeliveryMethod DeliveryMethod);
