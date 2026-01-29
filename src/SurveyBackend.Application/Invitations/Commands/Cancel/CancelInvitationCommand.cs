using System.Text.Json.Serialization;
using SurveyBackend.Application.Abstractions.Messaging;

namespace SurveyBackend.Application.Invitations.Commands.Cancel;

public sealed record CancelInvitationCommand(
    [property: JsonPropertyName("id")] int Id) : ICommand<bool>;
