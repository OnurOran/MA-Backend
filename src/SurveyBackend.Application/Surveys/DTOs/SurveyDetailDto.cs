using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyDetailDto(
    int Id,
    int SurveyNumber,
    string Slug,
    string Title,
    string? Description,
    string? IntroText,
    string? ConsentText,
    string? OutroText,
    AccessType AccessType,
    DateTime? StartDate,
    DateTime? EndDate,
    IReadOnlyCollection<SurveyQuestionDetailDto> Questions,
    AttachmentDto? Attachment = null);

public sealed record SurveyQuestionDetailDto(
    int Id,
    string Text,
    string? Description,
    QuestionType Type,
    int Order,
    bool IsRequired,
    IReadOnlyCollection<SurveyOptionDetailDto> Options,
    AttachmentDto? Attachment = null,
    IReadOnlyCollection<string>? AllowedAttachmentContentTypes = null,
    // Matrix question type properties
    IReadOnlyCollection<string>? MatrixScaleLabels = null,
    bool MatrixShowExplanation = false,
    string? MatrixExplanationLabel = null);

public sealed record SurveyOptionDetailDto(
    int Id,
    string Text,
    int Order,
    int? Value,
    AttachmentDto? Attachment = null,
    IReadOnlyCollection<SurveyQuestionDetailDto>? ChildQuestions = null);
