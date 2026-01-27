using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Commands.Update;

public sealed class UpdateSurveyCommandHandler : ICommandHandler<UpdateSurveyCommand, bool>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICurrentUserService _currentUserService;
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

    public UpdateSurveyCommandHandler(
        ISurveyRepository surveyRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IAttachmentService attachmentService)
    {
        _surveyRepository = surveyRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _attachmentService = attachmentService;
    }

    public async Task<bool> HandleAsync(UpdateSurveyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
                      ?? throw new InvalidOperationException("Anket bulunamadı.");

        var departmentId = existing.DepartmentId;
        await _authorizationService.EnsureDepartmentScopeAsync(departmentId, cancellationToken);

        // For published surveys, only allow text and isRequired changes
        if (existing.IsPublished)
        {
            return await HandlePublishedSurveyUpdateAsync(existing, request, cancellationToken);
        }

        var replacement = Survey.Create(
            existing.Slug,
            request.Title,
            request.Description,
            request.IntroText,
            request.ConsentText,
            request.OutroText,
            departmentId,
            request.AccessType);

        var questionAttachmentQueue = new List<(Question Question, AttachmentUploadDto Attachment)>();
        var optionAttachmentQueue = new List<(QuestionOption Option, AttachmentUploadDto Attachment)>();

        if (request.Questions is not null)
        {
            foreach (var questionDto in request.Questions)
            {
                if (questionDto.Type == Domain.Enums.QuestionType.FileUpload && questionDto.Options is not null && questionDto.Options.Count > 0)
                {
                    throw new InvalidOperationException("Dosya yükleme soruları için seçenek tanımlanamaz.");
                }
                if (questionDto.Type != Domain.Enums.QuestionType.FileUpload && questionDto.AllowedAttachmentContentTypes is not null && questionDto.AllowedAttachmentContentTypes.Count > 0)
                {
                    throw new InvalidOperationException("Dosya tipi kısıtı sadece dosya yükleme soruları için geçerlidir.");
                }

                if (questionDto.Type == Domain.Enums.QuestionType.Conditional)
                {
                    if (questionDto.Options is null || questionDto.Options.Count < 2 || questionDto.Options.Count > 5)
                    {
                        throw new InvalidOperationException("Koşullu sorular 2 ile 5 arasında seçenek içermelidir.");
                    }
                    if (questionDto.ChildQuestions is not null)
                    {
                        foreach (var childDto in questionDto.ChildQuestions)
                        {
                            if (childDto.Type == Domain.Enums.QuestionType.Conditional)
                            {
                                throw new InvalidOperationException("Alt sorular Koşullu tip olamaz.");
                            }
                            if (childDto.Type == Domain.Enums.QuestionType.Matrix)
                            {
                                throw new InvalidOperationException("Alt sorular Matris tip olamaz.");
                            }
                        }
                    }
                }

                if (questionDto.Type == Domain.Enums.QuestionType.SingleSelect || questionDto.Type == Domain.Enums.QuestionType.MultiSelect)
                {
                    if (questionDto.Options is null || questionDto.Options.Count < 2 || questionDto.Options.Count > 10)
                    {
                        throw new InvalidOperationException("Tek seçim ve çoklu seçim soruları 2 ile 10 arasında seçenek içermelidir.");
                    }
                }

                // Matrix question validation
                if (questionDto.Type == Domain.Enums.QuestionType.Matrix)
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

                var question = replacement.AddQuestion(questionDto.Text, questionDto.Type, questionDto.Order, questionDto.IsRequired);
                if (questionDto.Type == Domain.Enums.QuestionType.FileUpload)
                {
                    var normalizedAllowed = NormalizeAllowedContentTypes(questionDto.AllowedAttachmentContentTypes);
                    question.SetAllowedAttachmentContentTypes(normalizedAllowed);
                }

                // Set Matrix question properties
                if (questionDto.Type == Domain.Enums.QuestionType.Matrix)
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

                if (questionDto.Attachment is not null)
                {
                    questionAttachmentQueue.Add((question, questionDto.Attachment));
                }

                if (questionDto.Options is null)
                {
                    continue;
                }

                foreach (var optionDto in questionDto.Options)
                {
                    var option = question.AddOption(optionDto.Text, optionDto.Order, optionDto.Value);
                    if (optionDto.Attachment is not null)
                    {
                        optionAttachmentQueue.Add((option, optionDto.Attachment));
                    }
                }

                if (questionDto.Type == Domain.Enums.QuestionType.Conditional && questionDto.ChildQuestions is not null)
                {
                    foreach (var childDto in questionDto.ChildQuestions)
                    {
                        var parentOption = question.Options.FirstOrDefault(o => o.Order == childDto.ParentOptionOrder);
                        if (parentOption is null)
                        {
                            throw new InvalidOperationException($"Seçenek sırası {childDto.ParentOptionOrder} bulunamadı.");
                        }

                        var childQuestion = replacement.AddQuestion(childDto.Text, childDto.Type, childDto.Order, childDto.IsRequired);
                        if (childDto.Type == Domain.Enums.QuestionType.FileUpload)
                        {
                            var normalizedAllowed = NormalizeAllowedContentTypes(childDto.AllowedAttachmentContentTypes);
                            childQuestion.SetAllowedAttachmentContentTypes(normalizedAllowed);
                        }
                        if (childDto.Attachment is not null)
                        {
                            questionAttachmentQueue.Add((childQuestion, childDto.Attachment));
                        }

                        if (childDto.Options is not null)
                        {
                            foreach (var childOptionDto in childDto.Options)
                            {
                                var childOption = childQuestion.AddOption(childOptionDto.Text, childOptionDto.Order, childOptionDto.Value);
                                if (childOptionDto.Attachment is not null)
                                {
                                    optionAttachmentQueue.Add((childOption, childOptionDto.Attachment));
                                }
                            }
                        }

                        parentOption.AddDependentQuestion(childQuestion);
                    }
                }
            }
        }

        var attachmentsToRemove = CollectAttachments(existing);
        await _attachmentService.RemoveAttachmentsAsync(attachmentsToRemove, cancellationToken);
        await _surveyRepository.DeleteAsync(existing, cancellationToken);
        await _surveyRepository.AddAsync(replacement, cancellationToken);

        if (request.Attachment is not null)
        {
            await _attachmentService.SaveSurveyAttachmentAsync(replacement, request.Attachment, cancellationToken);
        }

        foreach (var pair in questionAttachmentQueue)
        {
            await _attachmentService.SaveQuestionAttachmentAsync(replacement, pair.Question, pair.Attachment, cancellationToken);
        }

        foreach (var pair in optionAttachmentQueue)
        {
            await _attachmentService.SaveOptionAttachmentAsync(replacement, pair.Option, pair.Attachment, cancellationToken);
        }

        return true;
    }

    private static List<Domain.Surveys.Attachment> CollectAttachments(Survey survey)
    {
        var list = new List<Domain.Surveys.Attachment>();

        if (survey.Attachment is not null)
        {
            list.Add(survey.Attachment);
        }

        foreach (var question in survey.Questions)
        {
            if (question.Attachment is not null)
            {
                list.Add(question.Attachment);
            }

            foreach (var option in question.Options)
            {
                if (option.Attachment is not null)
                {
                    list.Add(option.Attachment);
                }

                foreach (var dependent in option.DependentQuestions)
                {
                    if (dependent.ChildQuestion.Attachment is not null)
                    {
                        list.Add(dependent.ChildQuestion.Attachment);
                    }

                    foreach (var childOption in dependent.ChildQuestion.Options)
                    {
                        if (childOption.Attachment is not null)
                        {
                            list.Add(childOption.Attachment);
                        }
                    }
                }
            }
        }

        return list;
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

    /// <summary>
    /// Handles limited updates for published surveys.
    /// Only allows changing: survey metadata, question text/isRequired, option text, matrix labels.
    /// Does NOT allow: adding/removing questions, adding/removing options, changing types/order.
    /// </summary>
    private async Task<bool> HandlePublishedSurveyUpdateAsync(Survey existing, UpdateSurveyCommand request, CancellationToken cancellationToken)
    {
        // Validate structure hasn't changed
        if (request.Questions is null)
        {
            throw new InvalidOperationException("Yayınlanmış anketlerde soru listesi boş olamaz.");
        }

        // Get all questions including child questions for comparison
        var existingTopLevelQuestions = existing.Questions
            .Where(q => !existing.Questions
                .SelectMany(pq => pq.Options)
                .SelectMany(o => o.DependentQuestions)
                .Select(dq => dq.ChildQuestionId)
                .Contains(q.Id))
            .OrderBy(q => q.Order)
            .ToList();

        var requestTopLevelQuestions = request.Questions.OrderBy(q => q.Order).ToList();

        if (existingTopLevelQuestions.Count != requestTopLevelQuestions.Count)
        {
            throw new InvalidOperationException("Yayınlanmış anketlerde soru sayısı değiştirilemez.");
        }

        // Update survey metadata
        existing.Update(request.Title, request.Description, request.IntroText, request.ConsentText, request.OutroText, request.AccessType);

        // Update each question
        for (int i = 0; i < existingTopLevelQuestions.Count; i++)
        {
            var existingQuestion = existingTopLevelQuestions[i];
            var requestQuestion = requestTopLevelQuestions[i];

            // Validate type hasn't changed
            if (existingQuestion.Type != requestQuestion.Type)
            {
                throw new InvalidOperationException($"Yayınlanmış anketlerde soru tipi değiştirilemez (Soru {i + 1}).");
            }

            // Update question text and isRequired
            existingQuestion.UpdateTextContent(requestQuestion.Text, null, requestQuestion.IsRequired);

            // Update Matrix labels if applicable
            if (existingQuestion.Type == QuestionType.Matrix && requestQuestion.MatrixScaleLabels is not null)
            {
                if (requestQuestion.MatrixScaleLabels.Count == 5)
                {
                    existingQuestion.UpdateMatrixLabels(
                        requestQuestion.MatrixScaleLabels[0],
                        requestQuestion.MatrixScaleLabels[1],
                        requestQuestion.MatrixScaleLabels[2],
                        requestQuestion.MatrixScaleLabels[3],
                        requestQuestion.MatrixScaleLabels[4],
                        requestQuestion.MatrixExplanationLabel
                    );
                }
            }

            // Update options
            if (requestQuestion.Options is not null)
            {
                var existingOptions = existingQuestion.Options.OrderBy(o => o.Order).ToList();
                var requestOptions = requestQuestion.Options.OrderBy(o => o.Order).ToList();

                if (existingOptions.Count != requestOptions.Count)
                {
                    throw new InvalidOperationException($"Yayınlanmış anketlerde seçenek sayısı değiştirilemez (Soru {i + 1}).");
                }

                for (int j = 0; j < existingOptions.Count; j++)
                {
                    existingOptions[j].UpdateText(requestOptions[j].Text);
                }
            }

            // Handle child questions for Conditional type
            if (existingQuestion.Type == QuestionType.Conditional && requestQuestion.ChildQuestions is not null)
            {
                var existingChildQuestions = existingQuestion.Options
                    .SelectMany(o => o.DependentQuestions)
                    .Select(dq => dq.ChildQuestion)
                    .OrderBy(cq => cq.Order)
                    .ToList();

                var requestChildQuestions = requestQuestion.ChildQuestions.OrderBy(cq => cq.Order).ToList();

                if (existingChildQuestions.Count != requestChildQuestions.Count)
                {
                    throw new InvalidOperationException($"Yayınlanmış anketlerde alt soru sayısı değiştirilemez (Soru {i + 1}).");
                }

                for (int k = 0; k < existingChildQuestions.Count; k++)
                {
                    var existingChild = existingChildQuestions[k];
                    var requestChild = requestChildQuestions[k];

                    if (existingChild.Type != requestChild.Type)
                    {
                        throw new InvalidOperationException($"Yayınlanmış anketlerde alt soru tipi değiştirilemez (Soru {i + 1}, Alt soru {k + 1}).");
                    }

                    existingChild.UpdateTextContent(requestChild.Text, null, requestChild.IsRequired);

                    // Update child question options
                    if (requestChild.Options is not null)
                    {
                        var existingChildOptions = existingChild.Options.OrderBy(o => o.Order).ToList();
                        var requestChildOptions = requestChild.Options.OrderBy(o => o.Order).ToList();

                        if (existingChildOptions.Count != requestChildOptions.Count)
                        {
                            throw new InvalidOperationException($"Yayınlanmış anketlerde alt soru seçenek sayısı değiştirilemez (Soru {i + 1}, Alt soru {k + 1}).");
                        }

                        for (int l = 0; l < existingChildOptions.Count; l++)
                        {
                            existingChildOptions[l].UpdateText(requestChildOptions[l].Text);
                        }
                    }
                }
            }
        }

        await _surveyRepository.UpdateAsync(existing, cancellationToken);
        return true;
    }
}
