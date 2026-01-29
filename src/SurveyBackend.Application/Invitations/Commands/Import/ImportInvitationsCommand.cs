using System.Text.Json.Serialization;
using SurveyBackend.Application.Abstractions.Messaging;

namespace SurveyBackend.Application.Invitations.Commands.Import;

public sealed record ImportInvitationsCommand(
    [property: JsonPropertyName("surveyId")] int SurveyId,
    [property: JsonIgnore] Stream ExcelStream) : ICommand<int>;
