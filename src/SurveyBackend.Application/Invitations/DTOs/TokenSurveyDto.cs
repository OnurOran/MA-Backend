using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Invitations.DTOs;

public sealed record TokenSurveyDto(
    int SurveyId,
    int SurveyNumber,
    string Slug,
    string Title,
    string? Description,
    string? IntroText,
    string? ConsentText,
    string? OutroText,
    string FirstName,
    string LastName,
    bool HasParticipated,
    bool IsCompleted,
    DateTime? CompletedAt,
    int? ParticipationId,
    IReadOnlyCollection<SurveyQuestionDetailDto> Questions,
    AttachmentDto? Attachment);
