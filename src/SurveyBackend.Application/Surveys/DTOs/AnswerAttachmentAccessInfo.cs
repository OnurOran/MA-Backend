using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record AnswerAttachmentAccessInfo(
    int AttachmentId,
    int SurveyId,
    int DepartmentId,
    AccessType AccessType,
    bool IsActive,
    DateTime? StartDate,
    DateTime? EndDate,
    int ParticipationId,
    int AnswerId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string StoragePath);
