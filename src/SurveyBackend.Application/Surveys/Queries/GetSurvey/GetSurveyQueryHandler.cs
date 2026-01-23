using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Queries.GetSurvey;

public sealed class GetSurveyQueryHandler : ICommandHandler<GetSurveyQuery, SurveyDetailDto?>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSurveyQueryHandler(ISurveyRepository surveyRepository, ICurrentUserService currentUserService)
    {
        _surveyRepository = surveyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<SurveyDetailDto?> HandleAsync(GetSurveyQuery request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (survey is null)
        {
            return null;
        }

        if (!IsAvailable(survey) && !HasManagementAccess(survey))
        {
            return null;
        }

        var childQuestionIds = survey.Questions
            .SelectMany(q => q.Options)
            .SelectMany(o => o.DependentQuestions)
            .Select(dq => dq.ChildQuestionId)
            .ToHashSet();

        var questions = survey.Questions
            .Where(q => !childQuestionIds.Contains(q.Id))
            .OrderBy(q => q.Order)
            .Select(q => MapQuestion(q, survey))
            .ToList();

        return new SurveyDetailDto(
            survey.Id,
            survey.Id,
            survey.Slug,
            survey.Title,
            survey.Description,
            survey.IntroText,
            survey.ConsentText,
            survey.OutroText,
            survey.AccessType,
            survey.IsPublished,
            survey.StartDate,
            survey.EndDate,
            questions,
            MapAttachment(survey.Attachment));
    }

    private static bool IsAvailable(Survey survey)
    {
        var now = DateTime.Now;

        if (!survey.IsPublished)
        {
            return false;
        }

        if (survey.StartDate.HasValue && survey.StartDate.Value > now)
        {
            return false;
        }

        if (survey.EndDate.HasValue && survey.EndDate.Value <= now)
        {
            return false;
        }

        return true;
    }

    private static SurveyQuestionDetailDto MapQuestion(Question question, Survey survey)
    {
        var options = question.Options
            .OrderBy(o => o.Order)
            .Select(o => MapOption(o, survey))
            .ToList();

        return new SurveyQuestionDetailDto(
            question.Id,
            question.Text,
            question.Description,
            question.Type,
            question.Order,
            question.IsRequired,
            options,
            MapAttachment(question.Attachment),
            question.GetAllowedAttachmentContentTypes(),
            question.GetMatrixScaleLabels().All(s => string.IsNullOrEmpty(s)) ? null : question.GetMatrixScaleLabels(),
            question.MatrixShowExplanation,
            question.MatrixExplanationLabel);
    }

    private static SurveyOptionDetailDto MapOption(QuestionOption option, Survey survey)
    {

        IReadOnlyCollection<SurveyQuestionDetailDto>? childQuestions = null;
        if (option.DependentQuestions.Any())
        {
            childQuestions = option.DependentQuestions
                .OrderBy(dq => dq.ChildQuestion.Order)
                .Select(dq => MapChildQuestion(dq.ChildQuestion))
                .ToList();
        }

        return new SurveyOptionDetailDto(
            option.Id,
            option.Text,
            option.Order,
            option.Value,
            MapAttachment(option.Attachment),
            childQuestions);
    }

    private static SurveyQuestionDetailDto MapChildQuestion(Question question)
    {
        var options = question.Options
            .OrderBy(o => o.Order)
            .Select(o => new SurveyOptionDetailDto(
                o.Id,
                o.Text,
                o.Order,
                o.Value,
                MapAttachment(o.Attachment),
                null))
            .ToList();

        return new SurveyQuestionDetailDto(
            question.Id,
            question.Text,
            question.Description,
            question.Type,
            question.Order,
            question.IsRequired,
            options,
            MapAttachment(question.Attachment),
            question.GetAllowedAttachmentContentTypes(),
            question.GetMatrixScaleLabels().All(s => string.IsNullOrEmpty(s)) ? null : question.GetMatrixScaleLabels(),
            question.MatrixShowExplanation,
            question.MatrixExplanationLabel);
    }

    private static AttachmentDto? MapAttachment(Domain.Surveys.Attachment? attachment)
    {
        if (attachment is null)
        {
            return null;
        }

        return new AttachmentDto(attachment.Id, attachment.FileName, attachment.ContentType, attachment.SizeBytes);
    }

    private bool HasManagementAccess(Survey survey)
    {
        if (_currentUserService.IsSuperAdmin || _currentUserService.HasPermission("ManageUsers"))
        {
            return true;
        }

        if (_currentUserService.HasPermission("ManageDepartment") &&
            _currentUserService.DepartmentId.HasValue &&
            _currentUserService.DepartmentId.Value == survey.DepartmentId)
        {
            return true;
        }

        return false;
    }
}
