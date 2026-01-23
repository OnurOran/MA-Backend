using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandHandler : ICommandHandler<SubmitAnswerCommand, bool>
{
    private readonly IParticipationRepository _participationRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly AnswerAttachmentService _answerAttachmentService;

    public SubmitAnswerCommandHandler(
        IParticipationRepository participationRepository,
        ISurveyRepository surveyRepository,
        AnswerAttachmentService answerAttachmentService)
    {
        _participationRepository = participationRepository;
        _surveyRepository = surveyRepository;
        _answerAttachmentService = answerAttachmentService;
    }

    public async Task<bool> HandleAsync(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {

        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken)
                          ?? throw new InvalidOperationException("Katılım bulunamadı.");

        var survey = await _surveyRepository.GetByIdAsync(participation.SurveyId, cancellationToken)
                    ?? throw new InvalidOperationException("Anket bulunamadı.");

        var question = survey.Questions.FirstOrDefault(q => q.Id == request.QuestionId)
                       ?? throw new InvalidOperationException("Soru bulunamadı.");

        if (question.Type == QuestionType.FileUpload)
        {
            if (request.Attachment is null)
            {
                throw new InvalidOperationException("Bu soru için dosya yüklenmesi gereklidir.");
            }

            if (!string.IsNullOrWhiteSpace(request.TextValue) || (request.OptionIds is not null && request.OptionIds.Count > 0))
            {
                throw new InvalidOperationException("Dosya yükleme sorusu için metin veya seçenek gönderilemez.");
            }
        }
        else if (question.Type == QuestionType.Matrix)
        {
            if (request.MatrixAnswers is null || request.MatrixAnswers.Count == 0)
            {
                throw new InvalidOperationException("Matrix sorusu için en az bir cevap gereklidir.");
            }

            foreach (var matrixAnswer in request.MatrixAnswers)
            {
                if (matrixAnswer.ScaleValue < 1 || matrixAnswer.ScaleValue > 5)
                {
                    throw new InvalidOperationException("Matrix ölçek değeri 1-5 arasında olmalıdır.");
                }

                if (question.MatrixShowExplanation && matrixAnswer.ScaleValue <= 2 && string.IsNullOrWhiteSpace(matrixAnswer.Explanation))
                {
                    throw new InvalidOperationException("Düşük puanlarda açıklama zorunludur.");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.TextValue) || (request.OptionIds is not null && request.OptionIds.Count > 0) || request.Attachment is not null)
            {
                throw new InvalidOperationException("Matrix sorusu için yalnızca matrix cevapları gönderilebilir.");
            }
        }
        else if (request.Attachment is not null)
        {
            throw new InvalidOperationException("Dosya yalnızca dosya yükleme tipi sorularda gönderilebilir.");
        }

        var answer = participation.AddOrUpdateAnswer(
            request.QuestionId,
            question.Type == QuestionType.FileUpload || question.Type == QuestionType.Matrix ? null : request.TextValue,
            question.Type == QuestionType.FileUpload || question.Type == QuestionType.Matrix ? null : request.OptionIds);

        // Handle Matrix answers
        if (question.Type == QuestionType.Matrix && request.MatrixAnswers is not null)
        {
            var matrixAnswerTuples = request.MatrixAnswers
                .Select(ma => (ma.OptionId, ma.ScaleValue, ma.Explanation))
                .ToList();
            answer.ReplaceMatrixAnswers(matrixAnswerTuples);
        }

        await _participationRepository.UpdateAsync(participation, cancellationToken);

        if (question.Type == QuestionType.FileUpload && request.Attachment is not null)
        {
            await _answerAttachmentService.SaveAsync(survey, question, participation, answer, request.Attachment, cancellationToken);
        }

        return true;
    }
}
