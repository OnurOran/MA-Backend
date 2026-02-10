using ClosedXML.Excel;
using SurveyBackend.Application.Interfaces.Export;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Infrastructure.Export;

public sealed class ExcelExportService : IExcelExportService
{
    public byte[] GenerateSurveyReport(SurveyReportDto report)
    {
        using var workbook = new XLWorkbook();

        BuildSummarySheet(workbook, report);

        for (var i = 0; i < report.Questions.Count; i++)
        {
            var question = report.Questions[i];
            var sheetName = $"S{i + 1}";
            BuildQuestionSheet(workbook, sheetName, question, report.AccessType);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void BuildSummarySheet(IXLWorkbook workbook, SurveyReportDto report)
    {
        var ws = workbook.Worksheets.Add("Ozet");

        var row = 1;

        // Header
        ws.Cell(row, 1).Value = "Anket Raporu";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 16;
        row += 2;

        // Survey info
        row = AddInfoRow(ws, row, "Başlık", report.Title);
        row = AddInfoRow(ws, row, "Açıklama", report.Description);
        row = AddInfoRow(ws, row, "Erişim Tipi", report.AccessType);
        row = AddInfoRow(ws, row, "Başlangıç Tarihi", report.StartDate?.ToString("dd.MM.yyyy HH:mm") ?? "-");
        row = AddInfoRow(ws, row, "Bitiş Tarihi", report.EndDate?.ToString("dd.MM.yyyy HH:mm") ?? "-");
        row++;

        // Statistics
        ws.Cell(row, 1).Value = "İstatistikler";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 13;
        row++;

        row = AddInfoRow(ws, row, "Toplam Katılım", report.TotalParticipations.ToString());
        row = AddInfoRow(ws, row, "Tamamlanan", report.CompletedParticipations.ToString());
        row = AddInfoRow(ws, row, "Tamamlanma Oranı (%)", report.CompletionRate.ToString("F2"));
        row++;

        // Question index
        ws.Cell(row, 1).Value = "Soru Özeti";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 13;
        row++;

        var headerCols = new[] { "Sıra", "Soru Metni", "Soru Tipi", "Yanıt Sayısı", "Yanıt Oranı (%)", "Zorunlu" };
        for (var c = 0; c < headerCols.Length; c++)
        {
            ws.Cell(row, c + 1).Value = headerCols[c];
        }
        StyleHeaderRow(ws, row, headerCols.Length);
        row++;

        foreach (var q in report.Questions)
        {
            ws.Cell(row, 1).Value = q.Order;
            ws.Cell(row, 2).Value = q.Text;
            ws.Cell(row, 3).Value = GetQuestionTypeLabel(q.Type);
            ws.Cell(row, 4).Value = q.TotalResponses;
            ws.Cell(row, 5).Value = q.ResponseRate;
            ws.Cell(row, 6).Value = q.IsRequired ? "Evet" : "Hayır";
            row++;
        }
        row++;

        // Participants
        if (report.Participants.Count > 0)
        {
            ws.Cell(row, 1).Value = "Katılımcılar";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 13;
            row++;

            var pHeaders = new[] { "Katılımcı Adı", "Durum", "Başlangıç Tarihi" };
            for (var c = 0; c < pHeaders.Length; c++)
            {
                ws.Cell(row, c + 1).Value = pHeaders[c];
            }
            StyleHeaderRow(ws, row, pHeaders.Length);
            row++;

            foreach (var p in report.Participants)
            {
                ws.Cell(row, 1).Value = p.ParticipantName ?? "Anonim";
                ws.Cell(row, 2).Value = p.IsCompleted ? "Tamamlandı" : "Devam Ediyor";
                ws.Cell(row, 3).Value = p.StartedAt.ToString("dd.MM.yyyy HH:mm");
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildQuestionSheet(IXLWorkbook workbook, string sheetName, QuestionReportDto question, string accessType)
    {
        var ws = workbook.Worksheets.Add(sheetName);
        var row = 1;

        // Question header
        ws.Cell(row, 1).Value = question.Text;
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontSize = 14;
        row++;

        row = AddInfoRow(ws, row, "Soru Tipi", GetQuestionTypeLabel(question.Type));
        row = AddInfoRow(ws, row, "Yanıt Sayısı", question.TotalResponses.ToString());
        row = AddInfoRow(ws, row, "Yanıt Oranı (%)", question.ResponseRate.ToString("F2"));
        row++;

        switch (question.Type)
        {
            case "SingleSelect":
            case "MultiSelect":
                row = WriteOptionResults(ws, row, question.OptionResults);
                break;
            case "OpenText":
                row = WriteTextResponses(ws, row, question.TextResponses, accessType);
                break;
            case "FileUpload":
                row = WriteFileResponses(ws, row, question.FileResponses, accessType);
                break;
            case "Conditional":
                row = WriteConditionalResults(ws, row, question, accessType);
                break;
            case "Matrix":
                row = WriteMatrixResults(ws, row, question);
                break;
        }

        ws.Columns().AdjustToContents();
    }

    private static int WriteOptionResults(IXLWorksheet ws, int row, IReadOnlyList<OptionResultDto>? options)
    {
        if (options is null || options.Count == 0)
            return row;

        var headers = new[] { "Seçenek", "Seçim Sayısı", "Yüzde (%)" };
        for (var c = 0; c < headers.Length; c++)
        {
            ws.Cell(row, c + 1).Value = headers[c];
        }
        StyleHeaderRow(ws, row, headers.Length);
        row++;

        foreach (var opt in options)
        {
            ws.Cell(row, 1).Value = opt.Text;
            ws.Cell(row, 2).Value = opt.SelectionCount;
            ws.Cell(row, 3).Value = opt.Percentage;
            row++;
        }

        return row + 1;
    }

    private static int WriteTextResponses(IXLWorksheet ws, int row, IReadOnlyList<TextResponseDto>? responses, string accessType)
    {
        if (responses is null || responses.Count == 0)
            return row;

        var headers = new[] { "Katılımcı", "Yanıt", "Tarih" };
        for (var c = 0; c < headers.Length; c++)
        {
            ws.Cell(row, c + 1).Value = headers[c];
        }
        StyleHeaderRow(ws, row, headers.Length);
        row++;

        foreach (var r in responses)
        {
            ws.Cell(row, 1).Value = r.ParticipantName ?? "Anonim";
            ws.Cell(row, 2).Value = r.TextValue;
            ws.Cell(row, 3).Value = r.SubmittedAt.ToString("dd.MM.yyyy HH:mm");
            row++;
        }

        return row + 1;
    }

    private static int WriteFileResponses(IXLWorksheet ws, int row, IReadOnlyList<FileResponseDto>? responses, string accessType)
    {
        if (responses is null || responses.Count == 0)
            return row;

        var headers = new[] { "Katılımcı", "Dosya Adı", "Dosya Türü", "Boyut (KB)", "Tarih" };
        for (var c = 0; c < headers.Length; c++)
        {
            ws.Cell(row, c + 1).Value = headers[c];
        }
        StyleHeaderRow(ws, row, headers.Length);
        row++;

        foreach (var f in responses)
        {
            ws.Cell(row, 1).Value = f.ParticipantName ?? "Anonim";
            ws.Cell(row, 2).Value = f.FileName;
            ws.Cell(row, 3).Value = f.ContentType;
            ws.Cell(row, 4).Value = Math.Round(f.SizeBytes / 1024.0, 2);
            ws.Cell(row, 5).Value = f.SubmittedAt.ToString("dd.MM.yyyy HH:mm");
            row++;
        }

        return row + 1;
    }

    private static int WriteConditionalResults(IXLWorksheet ws, int row, QuestionReportDto question, string accessType)
    {
        // Parent option distribution
        row = WriteOptionResults(ws, row, question.OptionResults);

        if (question.ConditionalResults is null || question.ConditionalResults.Count == 0)
            return row;

        foreach (var branch in question.ConditionalResults)
        {
            ws.Cell(row, 1).Value = $"\"{branch.ParentOptionText}\" seçeneği ({branch.ParticipantCount} katılımcı)";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 12;
            row += 2;

            foreach (var child in branch.ChildQuestions)
            {
                ws.Cell(row, 1).Value = child.Text;
                ws.Cell(row, 1).Style.Font.Bold = true;
                row++;

                row = AddInfoRow(ws, row, "Soru Tipi", GetQuestionTypeLabel(child.Type));
                row = AddInfoRow(ws, row, "Yanıt Sayısı", child.TotalResponses.ToString());
                row++;

                switch (child.Type)
                {
                    case "SingleSelect":
                    case "MultiSelect":
                        row = WriteOptionResults(ws, row, child.OptionResults);
                        break;
                    case "OpenText":
                        row = WriteTextResponses(ws, row, child.TextResponses, accessType);
                        break;
                    case "FileUpload":
                        row = WriteFileResponses(ws, row, child.FileResponses, accessType);
                        break;
                    case "Matrix":
                        row = WriteMatrixResults(ws, row, child);
                        break;
                }
            }
        }

        return row;
    }

    private static int WriteMatrixResults(IXLWorksheet ws, int row, QuestionReportDto question)
    {
        if (question.MatrixResults is null || question.MatrixResults.Count == 0)
            return row;

        var scaleLabels = question.MatrixScaleLabels ?? new List<string> { "1", "2", "3", "4", "5" };
        var colCount = scaleLabels.Count + 3; // Madde + scales + Ortalama + Toplam Yanıt

        // Distribution table
        ws.Cell(row, 1).Value = "Madde";
        for (var i = 0; i < scaleLabels.Count; i++)
        {
            ws.Cell(row, i + 2).Value = scaleLabels[i];
        }
        ws.Cell(row, scaleLabels.Count + 2).Value = "Ortalama";
        ws.Cell(row, scaleLabels.Count + 3).Value = "Toplam Yanıt";
        StyleHeaderRow(ws, row, colCount);
        row++;

        foreach (var matrixRow in question.MatrixResults)
        {
            ws.Cell(row, 1).Value = matrixRow.Text;
            for (var i = 0; i < matrixRow.ScaleDistribution.Count && i < scaleLabels.Count; i++)
            {
                ws.Cell(row, i + 2).Value = matrixRow.ScaleDistribution[i];
            }
            ws.Cell(row, scaleLabels.Count + 2).Value = matrixRow.AverageScore;
            ws.Cell(row, scaleLabels.Count + 3).Value = matrixRow.TotalResponses;
            row++;
        }
        row++;

        // Explanations table
        var hasExplanations = question.MatrixResults.Any(r => r.Explanations.Count > 0);
        if (hasExplanations)
        {
            ws.Cell(row, 1).Value = "Açıklamalar";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 12;
            row++;

            var expHeaders = new[] { "Madde", "Katılımcı", "Puan", "Açıklama", "Tarih" };
            for (var c = 0; c < expHeaders.Length; c++)
            {
                ws.Cell(row, c + 1).Value = expHeaders[c];
            }
            StyleHeaderRow(ws, row, expHeaders.Length);
            row++;

            foreach (var matrixRow in question.MatrixResults)
            {
                foreach (var exp in matrixRow.Explanations)
                {
                    ws.Cell(row, 1).Value = matrixRow.Text;
                    ws.Cell(row, 2).Value = exp.ParticipantName ?? "Anonim";
                    ws.Cell(row, 3).Value = exp.ScaleValue;
                    ws.Cell(row, 4).Value = exp.Explanation;
                    ws.Cell(row, 5).Value = exp.SubmittedAt.ToString("dd.MM.yyyy HH:mm");
                    row++;
                }
            }
        }

        return row + 1;
    }

    private static int AddInfoRow(IXLWorksheet ws, int row, string label, string value)
    {
        ws.Cell(row, 1).Value = label;
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 2).Value = value;
        return row + 1;
    }

    private static void StyleHeaderRow(IXLWorksheet ws, int row, int colCount)
    {
        for (var c = 1; c <= colCount; c++)
        {
            var cell = ws.Cell(row, c);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }
    }

    private static string GetQuestionTypeLabel(string type) => type switch
    {
        "SingleSelect" => "Tek Seçim",
        "MultiSelect" => "Çoklu Seçim",
        "OpenText" => "Açık Metin",
        "FileUpload" => "Dosya Yükleme",
        "Conditional" => "Koşullu",
        "Matrix" => "Matris",
        _ => type
    };
}
