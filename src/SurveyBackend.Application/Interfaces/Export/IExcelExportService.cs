using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Interfaces.Export;

public interface IExcelExportService
{
    byte[] GenerateSurveyReport(SurveyReportDto report);
}
