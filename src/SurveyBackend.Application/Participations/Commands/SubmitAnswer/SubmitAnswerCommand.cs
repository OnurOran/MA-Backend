using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed record SubmitAnswerCommand(
    int ParticipationId,
    int QuestionId,
    string? TextValue,
    List<int>? OptionIds,
    AttachmentUploadDto? Attachment = null,
    List<MatrixAnswerItemDto>? MatrixAnswers = null) : ICommand<bool>;

public sealed record MatrixAnswerItemDto(
    int OptionId,
    int ScaleValue,
    string? Explanation);
