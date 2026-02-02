using System.Text.Json.Serialization;
using SurveyBackend.Application.Abstractions.Messaging;

namespace SurveyBackend.Application.Invitations.Commands.Resend;

public sealed record ResendInvitationsCommand(
    [property: JsonPropertyName("surveyId")] int SurveyId,
    [property: JsonPropertyName("baseUrl")] string BaseUrl) : ICommand<int>;
