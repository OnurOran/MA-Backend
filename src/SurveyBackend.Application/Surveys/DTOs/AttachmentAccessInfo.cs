using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record AttachmentAccessInfo(
    int AttachmentId,
    int SurveyId,
    int DepartmentId,
    AccessType AccessType,
    bool IsActive,
    DateTime? StartDate,
    DateTime? EndDate,
    string FileName,
    string ContentType,
    long SizeBytes,
    string StoragePath);
