using System.Text.Json.Serialization;
using SurveyBackend.Application.Abstractions.Messaging;

namespace SurveyBackend.Application.Invitations.Commands.Send;

public sealed record SendInvitationsCommand(
    [property: JsonPropertyName("surveyId")] int SurveyId,
    [property: JsonPropertyName("baseUrl")] string BaseUrl) : ICommand<int>;
