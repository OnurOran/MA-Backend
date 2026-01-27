using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Commands.Duplicate;

public sealed class DuplicateSurveyCommandHandler : ICommandHandler<DuplicateSurveyCommand, int>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public DuplicateSurveyCommandHandler(
        ISurveyRepository surveyRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _surveyRepository = surveyRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<int> HandleAsync(DuplicateSurveyCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
        }

        var departmentId = _currentUserService.DepartmentId
            ?? throw new UnauthorizedAccessException("Kullanıcının departman bilgisi bulunamadı.");

        // Fetch the original survey
        var originalSurvey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken)
            ?? throw new InvalidOperationException($"Anket bulunamadı: {request.SurveyId}");

        // Verify user has access to the survey's department
        await _authorizationService.EnsureDepartmentScopeAsync(originalSurvey.DepartmentId, cancellationToken);

        // Generate slug from the original title
        var baseSlug = Survey.GenerateSlug(originalSurvey.Title);

        // Create the duplicated survey with the same properties but as a draft (not published)
        var duplicatedSurvey = Survey.Create(
            baseSlug,
            originalSurvey.Title,
            originalSurvey.Description,
            originalSurvey.IntroText,
            originalSurvey.ConsentText,
            originalSurvey.OutroText,
            originalSurvey.DepartmentId,
            originalSurvey.AccessType);

        // Keep track of original question IDs to new question mappings
        // This is needed for handling dependent questions in conditional types
        var questionMapping = new Dictionary<int, Question>();

        // First pass: Copy all questions and their options (but not dependent questions yet)
        foreach (var originalQuestion in originalSurvey.Questions.OrderBy(q => q.Order))
        {
            var newQuestion = duplicatedSurvey.AddQuestion(
                originalQuestion.Text,
                originalQuestion.Type,
                originalQuestion.Order,
                originalQuestion.IsRequired,
                originalQuestion.Description);

            // Copy allowed attachment content types for FileUpload questions
            if (originalQuestion.Type == QuestionType.FileUpload)
            {
                var allowedTypes = originalQuestion.GetAllowedAttachmentContentTypes();
                if (allowedTypes.Count > 0)
                {
                    newQuestion.SetAllowedAttachmentContentTypes(allowedTypes);
                }
            }

            // Copy Matrix question properties
            if (originalQuestion.Type == QuestionType.Matrix)
            {
                var scaleLabels = originalQuestion.GetMatrixScaleLabels();
                newQuestion.SetMatrixScaleLabels(scaleLabels[0], scaleLabels[1], scaleLabels[2], scaleLabels[3], scaleLabels[4]);
                newQuestion.SetMatrixExplanationSettings(originalQuestion.MatrixShowExplanation, originalQuestion.MatrixExplanationLabel);
            }

            // Store the mapping
            questionMapping[originalQuestion.Id] = newQuestion;

            // Copy options
            foreach (var originalOption in originalQuestion.Options.OrderBy(o => o.Order))
            {
                newQuestion.AddOption(
                    originalOption.Text,
                    originalOption.Order,
                    originalOption.Value);
            }
        }

        // Save the survey to get the ID
        await _surveyRepository.AddAsync(duplicatedSurvey, cancellationToken);

        // Update slug with the new survey ID
        var finalSlug = $"{baseSlug}-{duplicatedSurvey.Id}";
        duplicatedSurvey.UpdateSlug(finalSlug);
        await _surveyRepository.UpdateAsync(duplicatedSurvey, cancellationToken);

        // Second pass: Handle dependent questions for conditional question types
        // We need to do this after all questions are created and have IDs
        foreach (var originalQuestion in originalSurvey.Questions.Where(q => q.Type == QuestionType.Conditional))
        {
            var newQuestion = questionMapping[originalQuestion.Id];

            foreach (var originalOption in originalQuestion.Options.OrderBy(o => o.Order))
            {
                // Find the corresponding new option by order
                var newOption = newQuestion.Options.FirstOrDefault(o => o.Order == originalOption.Order);
                if (newOption == null) continue;

                // Copy dependent questions
                foreach (var dependentQuestion in originalOption.DependentQuestions)
                {
                    // Find the new child question using the mapping
                    if (questionMapping.TryGetValue(dependentQuestion.ChildQuestionId, out var newChildQuestion))
                    {
                        newOption.AddDependentQuestion(newChildQuestion);
                    }
                }
            }
        }

        // Save the dependent questions relationships
        await _surveyRepository.UpdateAsync(duplicatedSurvey, cancellationToken);

        return duplicatedSurvey.Id;
    }
}
