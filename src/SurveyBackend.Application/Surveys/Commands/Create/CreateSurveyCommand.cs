using System.Text.Json.Serialization;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.Commands.Create;

public sealed record CreateSurveyCommand(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("introText")] string? IntroText,
    [property: JsonPropertyName("consentText")] string? ConsentText,
    [property: JsonPropertyName("outroText")] string? OutroText,
    [property: JsonPropertyName("accessType")] AccessType AccessType,
    [property: JsonPropertyName("questions")] List<CreateQuestionDto>? Questions,
    [property: JsonPropertyName("attachment")] AttachmentUploadDto? Attachment = null) : ICommand<int>;
