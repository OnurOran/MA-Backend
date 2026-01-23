using SurveyBackend.Application.Participations.Commands.SubmitAnswer;
using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Participations.DTOs;

public sealed record SubmitAnswerRequest(
    int QuestionId,
    string? TextValue,
    List<int>? OptionIds,
    AttachmentUploadDto? Attachment = null,
    List<MatrixAnswerItemDto>? MatrixAnswers = null);
