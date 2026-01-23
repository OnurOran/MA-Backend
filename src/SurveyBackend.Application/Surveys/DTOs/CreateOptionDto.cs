using System.Text.Json.Serialization;

namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record CreateOptionDto(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("order")] int Order,
    [property: JsonPropertyName("value")] int? Value,
    [property: JsonPropertyName("attachment")] AttachmentUploadDto? Attachment = null);
