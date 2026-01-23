using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.Commands.Update;

public sealed record UpdateSurveyCommand(
    int SurveyId,
    string Title,
    string? Description,
    string? IntroText,
    string? ConsentText,
    string? OutroText,
    AccessType AccessType,
    List<CreateQuestionDto>? Questions,
    AttachmentUploadDto? Attachment = null) : ICommand<bool>;
