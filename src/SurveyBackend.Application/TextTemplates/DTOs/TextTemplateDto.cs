using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.TextTemplates.DTOs;

public sealed record TextTemplateDto(
    int Id,
    string Title,
    string Content,
    TextTemplateType Type,
    int DepartmentId,
    DateTime CreateDate);
