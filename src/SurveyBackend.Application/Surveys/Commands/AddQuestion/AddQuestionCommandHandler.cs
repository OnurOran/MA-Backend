using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.Commands.AddQuestion;

public sealed class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand, int>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAttachmentService _attachmentService;
    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/webp",
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.ms-powerpoint"
        };

    public AddQuestionCommandHandler(
        ISurveyRepository surveyRepository,
        IAuthorizationService authorizationService,
        IAttachmentService attachmentService)
    {
        _surveyRepository = surveyRepository;
        _authorizationService = authorizationService;
        _attachmentService = attachmentService;
    }

    public async Task<int> HandleAsync(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                     ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

        var questionDto = request.Question ?? throw new ArgumentNullException(nameof(request.Question));
        if (questionDto.Type == QuestionType.FileUpload && questionDto.Options is not null && questionDto.Options.Count > 0)
        {
            throw new InvalidOperationException("Dosya yükleme soruları için seçenek tanımlanamaz.");
        }
        if (questionDto.Type != QuestionType.FileUpload && questionDto.AllowedAttachmentContentTypes is not null && questionDto.AllowedAttachmentContentTypes.Count > 0)
        {
            throw new InvalidOperationException("Dosya tipi kısıtı sadece dosya yükleme soruları için geçerlidir.");
        }

        // Matrix question validation
        if (questionDto.Type == QuestionType.Matrix)
        {
            if (questionDto.MatrixScaleLabels is null || questionDto.MatrixScaleLabels.Count != 5)
            {
                throw new InvalidOperationException("Matrix soruları için 5 adet ölçek etiketi gereklidir.");
            }
            if (questionDto.MatrixScaleLabels.Any(label => string.IsNullOrWhiteSpace(label)))
            {
                throw new InvalidOperationException("Tüm ölçek etiketleri doldurulmalıdır.");
            }
            if (questionDto.Options is null || questionDto.Options.Count < 2 || questionDto.Options.Count > 20)
            {
                throw new InvalidOperationException("Matrix soruları için 2-20 arasında satır (seçenek) gereklidir.");
            }
            if (questionDto.MatrixShowExplanation && string.IsNullOrWhiteSpace(questionDto.MatrixExplanationLabel))
            {
                throw new InvalidOperationException("Açıklama etiketi zorunludur.");
            }
        }

        var question = survey.AddQuestion(questionDto.Text, questionDto.Type, questionDto.Order, questionDto.IsRequired);
        if (questionDto.Type == QuestionType.FileUpload)
        {
            var normalizedAllowed = NormalizeAllowedContentTypes(questionDto.AllowedAttachmentContentTypes);
            question.SetAllowedAttachmentContentTypes(normalizedAllowed);
        }

        // Set Matrix question properties
        if (questionDto.Type == QuestionType.Matrix)
        {
            if (questionDto.MatrixScaleLabels is not null && questionDto.MatrixScaleLabels.Count == 5)
            {
                question.SetMatrixScaleLabels(
                    questionDto.MatrixScaleLabels[0],
                    questionDto.MatrixScaleLabels[1],
                    questionDto.MatrixScaleLabels[2],
                    questionDto.MatrixScaleLabels[3],
                    questionDto.MatrixScaleLabels[4]
                );
            }
            // Always set explanation settings for Matrix questions
            question.SetMatrixExplanationSettings(questionDto.MatrixShowExplanation, questionDto.MatrixExplanationLabel);
        }

        var optionAttachmentQueue = new List<(QuestionOption Option, AttachmentUploadDto Attachment)>();
        if (questionDto.Options is not null)
        {
            foreach (var option in questionDto.Options)
            {
                var createdOption = question.AddOption(option.Text, option.Order, option.Value);
                if (option.Attachment is not null)
                {
                    optionAttachmentQueue.Add((createdOption, option.Attachment));
                }
            }
        }

        await _surveyRepository.UpdateAsync(survey, cancellationToken);

        if (questionDto.Attachment is not null)
        {
            await _attachmentService.SaveQuestionAttachmentAsync(survey, question, questionDto.Attachment, cancellationToken);
        }

        foreach (var pair in optionAttachmentQueue)
        {
            await _attachmentService.SaveOptionAttachmentAsync(survey, pair.Option, pair.Attachment, cancellationToken);
        }
        return question.Id;
    }

    private static List<string>? NormalizeAllowedContentTypes(IEnumerable<string>? contentTypes)
    {
        if (contentTypes is null)
        {
            return null;
        }

        var list = contentTypes
            .Where(ct => !string.IsNullOrWhiteSpace(ct))
            .Select(ct => ct.Trim())
            .Where(ct => AllowedContentTypes.Contains(ct, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return list.Count > 0 ? list : null;
    }
}
