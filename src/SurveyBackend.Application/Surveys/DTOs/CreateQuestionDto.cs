using System.Text.Json.Serialization;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record CreateQuestionDto(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("type")] QuestionType Type,
    [property: JsonPropertyName("order")] int Order,
    [property: JsonPropertyName("isRequired")] bool IsRequired,
    [property: JsonPropertyName("options")] List<CreateOptionDto>? Options,
    [property: JsonPropertyName("attachment")] AttachmentUploadDto? Attachment = null,
    [property: JsonPropertyName("allowedAttachmentContentTypes")] List<string>? AllowedAttachmentContentTypes = null,
    [property: JsonPropertyName("childQuestions")] List<CreateChildQuestionDto>? ChildQuestions = null,
    // Matrix question type properties
    [property: JsonPropertyName("matrixScaleLabels")] List<string>? MatrixScaleLabels = null,
    [property: JsonPropertyName("matrixShowExplanation")] bool MatrixShowExplanation = false,
    [property: JsonPropertyName("matrixExplanationLabel")] string? MatrixExplanationLabel = null);

public sealed record CreateChildQuestionDto(
    [property: JsonPropertyName("parentOptionOrder")] int ParentOptionOrder,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("type")] QuestionType Type,
    [property: JsonPropertyName("order")] int Order,
    [property: JsonPropertyName("isRequired")] bool IsRequired,
    [property: JsonPropertyName("options")] List<CreateOptionDto>? Options,
    [property: JsonPropertyName("attachment")] AttachmentUploadDto? Attachment = null,
    [property: JsonPropertyName("allowedAttachmentContentTypes")] List<string>? AllowedAttachmentContentTypes = null);
