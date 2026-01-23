using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyListItemDto(
    int Id,
    int SurveyNumber,
    string Slug,
    string Title,
    string? Description,
    int DepartmentId,
    AccessType AccessType,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? StartDate,
    DateTime? EndDate,
    string CreatedBy);
