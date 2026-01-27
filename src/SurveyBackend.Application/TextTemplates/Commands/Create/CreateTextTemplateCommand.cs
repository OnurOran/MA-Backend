using System.Text.Json.Serialization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.TextTemplates.Commands.Create;

public sealed record CreateTextTemplateCommand(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("type")] TextTemplateType Type) : ICommand<int>;
