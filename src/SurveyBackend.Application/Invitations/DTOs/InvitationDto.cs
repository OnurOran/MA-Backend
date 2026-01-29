using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Invitations.DTOs;

public sealed record InvitationDto(
    int Id,
    int SurveyId,
    string Token,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DeliveryMethod DeliveryMethod,
    InvitationStatus Status,
    DateTime? SentAt,
    DateTime? ViewedAt,
    DateTime? CompletedAt,
    int? ParticipationId,
    DateTime CreateDate);
