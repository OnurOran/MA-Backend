using ClosedXML.Excel;
using SurveyBackend.Application.Interfaces.Import;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Infrastructure.Import;

public sealed class ExcelInvitationImportService : IExcelImportService
{
    public Task<IReadOnlyList<InvitationImportRow>> ParseInvitationsAsync(
        Stream excelStream,
        CancellationToken cancellationToken)
    {
        var invitations = new List<InvitationImportRow>();

        using var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.First();

        var headerRow = worksheet.Row(1);
        var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var col = 1; col <= headerRow.LastCellUsed()?.Address.ColumnNumber; col++)
        {
            var headerValue = headerRow.Cell(col).GetString().Trim();
            if (!string.IsNullOrEmpty(headerValue))
            {
                columnMap[headerValue] = col;
            }
        }

        var firstNameCol = GetColumnIndex(columnMap, "FirstName", "Ad", "İsim");
        var lastNameCol = GetColumnIndex(columnMap, "LastName", "Soyad", "Soyisim");
        var emailCol = GetColumnIndex(columnMap, "Email", "E-posta", "Eposta", "Mail");
        var phoneCol = GetColumnIndex(columnMap, "Phone", "Telefon", "Tel", "Cep");
        var deliveryCol = GetColumnIndex(columnMap, "DeliveryMethod", "Gönderim", "Yöntem", "Delivery");

        if (firstNameCol == 0 || lastNameCol == 0)
        {
            throw new InvalidOperationException(
                "Excel dosyasında 'FirstName'/'Ad' ve 'LastName'/'Soyad' sütunları zorunludur.");
        }

        var lastRowNumber = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var row = 2; row <= lastRowNumber; row++)
        {
            var worksheetRow = worksheet.Row(row);

            var firstName = worksheetRow.Cell(firstNameCol).GetString().Trim();
            var lastName = worksheetRow.Cell(lastNameCol).GetString().Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                continue;
            }

            var email = emailCol > 0 ? worksheetRow.Cell(emailCol).GetString().Trim() : null;
            var phone = phoneCol > 0 ? worksheetRow.Cell(phoneCol).GetString().Trim() : null;
            var deliveryStr = deliveryCol > 0 ? worksheetRow.Cell(deliveryCol).GetString().Trim() : null;

            var deliveryMethod = ParseDeliveryMethod(deliveryStr, email, phone);

            if (deliveryMethod == DeliveryMethod.Email && string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException(
                    $"Satır {row}: Email gönderim yöntemi seçildi ancak email adresi boş.");
            }

            if (deliveryMethod == DeliveryMethod.Sms && string.IsNullOrEmpty(phone))
            {
                throw new InvalidOperationException(
                    $"Satır {row}: SMS gönderim yöntemi seçildi ancak telefon numarası boş.");
            }

            invitations.Add(new InvitationImportRow(
                firstName,
                lastName,
                string.IsNullOrEmpty(email) ? null : email,
                string.IsNullOrEmpty(phone) ? null : phone,
                deliveryMethod));
        }

        return Task.FromResult<IReadOnlyList<InvitationImportRow>>(invitations);
    }

    private static int GetColumnIndex(Dictionary<string, int> columnMap, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            if (columnMap.TryGetValue(name, out var index))
            {
                return index;
            }
        }
        return 0;
    }

    private static DeliveryMethod ParseDeliveryMethod(string? deliveryStr, string? email, string? phone)
    {
        if (!string.IsNullOrEmpty(deliveryStr))
        {
            if (deliveryStr.Equals("Sms", StringComparison.OrdinalIgnoreCase) ||
                deliveryStr.Equals("SMS", StringComparison.OrdinalIgnoreCase))
            {
                return DeliveryMethod.Sms;
            }

            if (deliveryStr.Equals("Email", StringComparison.OrdinalIgnoreCase) ||
                deliveryStr.Equals("E-posta", StringComparison.OrdinalIgnoreCase))
            {
                return DeliveryMethod.Email;
            }
        }

        if (!string.IsNullOrEmpty(email))
        {
            return DeliveryMethod.Email;
        }

        if (!string.IsNullOrEmpty(phone))
        {
            return DeliveryMethod.Sms;
        }

        throw new InvalidOperationException(
            "Gönderim yöntemi belirlenemedi. Email veya telefon numarası zorunludur.");
    }
}
