using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveyReport;

public sealed class GetParticipantResponseQueryHandler : ICommandHandler<GetParticipantResponseQuery, ParticipantResponseDto?>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetParticipantResponseQueryHandler(
        ISurveyRepository surveyRepository,
        IParticipationRepository participationRepository,
        ICurrentUserService currentUserService)
    {
        _surveyRepository = surveyRepository;
        _participationRepository = participationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ParticipantResponseDto?> HandleAsync(GetParticipantResponseQuery request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken);
        if (survey is null)
        {
            return null;
        }

        if (survey.AccessType != AccessType.Internal)
        {
            return null;
        }

        if (!HasManagementAccess(survey))
        {
            return null;
        }

        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken);

        if (participation is null || participation.SurveyId != request.SurveyId)
        {
            return null;
        }

        var answers = participation.Answers
            .Select(a => new ParticipantAnswerDto
            {
                QuestionId = a.QuestionId,
                QuestionText = a.Question.Text,
                TextValue = a.TextValue,
                SelectedOptions = a.Question.Type == QuestionType.Matrix
                    ? Array.Empty<string>()
                    : a.SelectedOptions
                        .OrderBy(so => so.QuestionOption.Order)
                        .Select(so => so.QuestionOption.Text)
                        .ToList(),
                FileName = a.Attachment?.FileName,
                AnswerId = a.Attachment != null ? a.Attachment.Id : null,
                MatrixAnswers = a.Question.Type == QuestionType.Matrix
                    ? a.SelectedOptions
                        .Where(so => so.ScaleValue.HasValue)
                        .OrderBy(so => so.QuestionOption.Order)
                        .Select(so => new MatrixAnswerDetailDto
                        {
                            RowText = so.QuestionOption.Text,
                            ScaleValue = so.ScaleValue ?? 0,
                            Explanation = so.Explanation
                        })
                        .ToList()
                    : null
            })
            .ToList();

        return new ParticipantResponseDto
        {
            ParticipationId = participation.Id,
            ParticipantName = participation.Participant?.LdapUsername,
            IsCompleted = participation.CompletedAt.HasValue,
            StartedAt = participation.StartedAt,
            CompletedAt = participation.CompletedAt,
            Answers = answers
        };
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
