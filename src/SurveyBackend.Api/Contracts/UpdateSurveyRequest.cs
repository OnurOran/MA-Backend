using System.Text.Json.Serialization;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Api.Contracts;

public sealed class UpdateSurveyRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("introText")]
    public string? IntroText { get; set; }

    [JsonPropertyName("consentText")]
    public string? ConsentText { get; set; }

    [JsonPropertyName("outroText")]
    public string? OutroText { get; set; }

    [JsonPropertyName("accessType")]
    public AccessType AccessType { get; set; }

    [JsonPropertyName("questions")]
    public List<CreateQuestionDto>? Questions { get; set; }

    [JsonPropertyName("attachment")]
    public AttachmentUploadDto? Attachment { get; set; }
}
