using System.Text.Json.Serialization;
using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Invitations.Commands.Create;

public sealed record CreateInvitationCommand(
    [property: JsonPropertyName("surveyId")] int SurveyId,
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string LastName,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("phone")] string? Phone,
    [property: JsonPropertyName("deliveryMethod")] DeliveryMethod DeliveryMethod) : ICommand<int>;
