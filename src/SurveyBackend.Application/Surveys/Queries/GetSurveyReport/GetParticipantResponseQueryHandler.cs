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
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetParticipantResponseQueryHandler(
        ISurveyRepository surveyRepository,
        IParticipationRepository participationRepository,
        ISurveyInvitationRepository invitationRepository,
        ICurrentUserService currentUserService)
    {
        _surveyRepository = surveyRepository;
        _participationRepository = participationRepository;
        _invitationRepository = invitationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ParticipantResponseDto?> HandleAsync(GetParticipantResponseQuery request, CancellationToken cancellationToken)
    {
        var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken);
        if (survey is null)
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

        // Resolve participant name based on access type
        string? participantName = null;
        if (survey.AccessType == AccessType.Internal)
        {
            participantName = participation.Participant?.LdapUsername;
        }
        else if (survey.AccessType == AccessType.InvitationOnly)
        {
            var invitations = await _invitationRepository.GetBySurveyIdAsync(request.SurveyId, cancellationToken);
            var invitation = invitations.FirstOrDefault(i => i.ParticipationId == participation.Id);
            if (invitation != null)
            {
                participantName = invitation.GetFullName();
            }
            else if (participation.Participant?.InvitationId.HasValue == true)
            {
                var linkedInvitation = invitations.FirstOrDefault(i => i.Id == participation.Participant.InvitationId.Value);
                participantName = linkedInvitation?.GetFullName();
            }
        }
        else if (survey.AccessType == AccessType.Public)
        {
            var allParticipations = await _participationRepository.GetBySurveyIdAsync(request.SurveyId, cancellationToken);
            var orderedParticipations = allParticipations.OrderBy(p => p.StartedAt).ToList();
            var index = orderedParticipations.FindIndex(p => p.Id == participation.Id);
            participantName = index >= 0 ? $"Katılımcı #{index + 1}" : null;
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
            ParticipantName = participantName,
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
