using SurveyBackend.Application.Common;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Queries.GetSurveyReport;

public sealed class GetSurveyReportQueryHandler : ICommandHandler<GetSurveyReportQuery, SurveyReportDto?>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IParticipationRepository _participationRepository;
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSurveyReportQueryHandler(
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

    public async Task<SurveyReportDto?> HandleAsync(GetSurveyReportQuery request, CancellationToken cancellationToken)
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

        var allParticipations = await _participationRepository.GetBySurveyIdAsync(request.SurveyId, cancellationToken);

        // Filter participations based on completion status
        var participations = request.IncludePartialResponses
            ? allParticipations
            : allParticipations.Where(p => p.CompletedAt.HasValue).ToList();

        var totalParticipations = allParticipations.Count;
        var completedParticipations = allParticipations.Count(p => p.CompletedAt.HasValue);
        var completionRate = totalParticipations > 0 ? (double)completedParticipations / totalParticipations * 100 : 0;

        // Build invitation lookup for InvitationOnly surveys
        Dictionary<int, SurveyInvitation>? invitationLookup = null;
        if (survey.AccessType == AccessType.InvitationOnly)
        {
            var invitations = await _invitationRepository.GetBySurveyIdAsync(request.SurveyId, cancellationToken);
            invitationLookup = invitations
                .Where(i => i.ParticipationId.HasValue)
                .ToDictionary(i => i.ParticipationId!.Value, i => i);
        }

        var showParticipantNames = survey.AccessType == AccessType.Internal || survey.AccessType == AccessType.InvitationOnly;

        var participantList = showParticipantNames
            ? participations.Select(p => new ParticipantSummaryDto
            {
                ParticipationId = p.Id,
                ParticipantName = GetParticipantName(p, survey.AccessType, invitationLookup),
                IsCompleted = p.CompletedAt.HasValue,
                StartedAt = p.StartedAt
            }).ToList()
            : new List<ParticipantSummaryDto>();

        var childQuestionIds = survey.Questions
            .SelectMany(q => q.Options)
            .SelectMany(o => o.DependentQuestions)
            .Select(dq => dq.ChildQuestionId)
            .ToHashSet();

        var questionReports = survey.Questions
            .Where(q => !childQuestionIds.Contains(q.Id))
            .OrderBy(q => q.Order)
            .Select(q => BuildQuestionReport(q, participations, showParticipantNames, invitationLookup))
            .ToList();

        return new SurveyReportDto
        {
            SurveyId = survey.Id,
            Title = survey.Title,
            Description = survey.Description ?? string.Empty,
            IntroText = survey.IntroText,
            OutroText = survey.OutroText,
            AccessType = survey.AccessType.ToString(),
            StartDate = survey.StartDate,
            EndDate = survey.EndDate,
            IsActive = survey.IsPublished,
            TotalParticipations = totalParticipations,
            CompletedParticipations = completedParticipations,
            CompletionRate = Math.Round(completionRate, 2),
            Participants = participantList,
            Questions = questionReports,
            Attachment = survey.Attachment != null ? new AttachmentDto(
                survey.Attachment.Id,
                survey.Attachment.FileName,
                survey.Attachment.ContentType,
                survey.Attachment.SizeBytes
            ) : null
        };
    }

    private static string? GetParticipantName(
        Participation participation,
        AccessType accessType,
        Dictionary<int, SurveyInvitation>? invitationLookup)
    {
        if (accessType == AccessType.Internal)
        {
            return participation.Participant?.LdapUsername ?? "Unknown";
        }

        if (accessType == AccessType.InvitationOnly && invitationLookup != null)
        {
            if (invitationLookup.TryGetValue(participation.Id, out var invitation))
            {
                return invitation.GetFullName();
            }
            // Fallback to participant's invitation if linked
            if (participation.Participant?.InvitationId.HasValue == true)
            {
                var linkedInvitation = invitationLookup.Values
                    .FirstOrDefault(i => i.Id == participation.Participant.InvitationId.Value);
                if (linkedInvitation != null)
                {
                    return linkedInvitation.GetFullName();
                }
            }
        }

        return null;
    }

    private static string? GetParticipantNameForParticipation(
        Participation participation,
        Dictionary<int, SurveyInvitation>? invitationLookup)
    {
        // First check if we have an invitation lookup
        if (invitationLookup != null && invitationLookup.TryGetValue(participation.Id, out var invitation))
        {
            return invitation.GetFullName();
        }

        // Check linked invitation via participant
        if (invitationLookup != null && participation.Participant?.InvitationId.HasValue == true)
        {
            var linkedInvitation = invitationLookup.Values
                .FirstOrDefault(i => i.Id == participation.Participant.InvitationId.Value);
            if (linkedInvitation != null)
            {
                return linkedInvitation.GetFullName();
            }
        }

        // Fallback to LDAP username for internal surveys
        return participation.Participant?.LdapUsername;
    }

    private QuestionReportDto BuildQuestionReport(
        Question question,
        IReadOnlyList<Participation> participations,
        bool showParticipantNames,
        Dictionary<int, SurveyInvitation>? invitationLookup)
    {
        var answers = participations
            .SelectMany(p => p.Answers)
            .Where(a => a.QuestionId == question.Id)
            .ToList();

        var totalResponses = answers.Count;
        var totalParticipations = participations.Count;
        var responseRate = totalParticipations > 0 ? (double)totalResponses / totalParticipations * 100 : 0;

        QuestionReportDto report = question.Type switch
        {
            QuestionType.SingleSelect => BuildSingleSelectReport(question, answers, totalResponses, responseRate),
            QuestionType.MultiSelect => BuildMultiSelectReport(question, answers, totalResponses, responseRate),
            QuestionType.OpenText => BuildOpenTextReport(question, answers, participations, totalResponses, responseRate, showParticipantNames, invitationLookup),
            QuestionType.FileUpload => BuildFileUploadReport(question, answers, participations, totalResponses, responseRate, showParticipantNames, invitationLookup),
            QuestionType.Conditional => BuildConditionalReport(question, participations, totalResponses, responseRate, showParticipantNames, invitationLookup),
            QuestionType.Matrix => BuildMatrixReport(question, answers, participations, totalResponses, responseRate, showParticipantNames, invitationLookup),
            _ => new QuestionReportDto
            {
                QuestionId = question.Id,
                Text = question.Text,
                Type = question.Type.ToString(),
                Order = question.Order,
                IsRequired = question.IsRequired,
                TotalResponses = totalResponses,
                ResponseRate = Math.Round(responseRate, 2),
                Attachment = question.Attachment != null ? new AttachmentDto(
                    question.Attachment.Id,
                    question.Attachment.FileName,
                    question.Attachment.ContentType,
                    question.Attachment.SizeBytes
                ) : null
            }
        };

        return report;
    }

    private QuestionReportDto BuildSingleSelectReport(Question question, List<Answer> answers, int totalResponses, double responseRate)
    {
        var optionResults = question.Options
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                var selectionCount = answers.Count(a => a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id));
                var percentage = totalResponses > 0 ? (double)selectionCount / totalResponses * 100 : 0;

                return new OptionResultDto
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Order = option.Order,
                    SelectionCount = selectionCount,
                    Percentage = Math.Round(percentage, 2),
                    Attachment = option.Attachment != null ? new AttachmentDto(
                        option.Attachment.Id,
                        option.Attachment.FileName,
                        option.Attachment.ContentType,
                        option.Attachment.SizeBytes
                    ) : null
                };
            })
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            OptionResults = optionResults
        };
    }

    private QuestionReportDto BuildMultiSelectReport(Question question, List<Answer> answers, int totalResponses, double responseRate)
    {

        var optionResults = question.Options
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                var selectionCount = answers.Count(a => a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id));

                var percentage = totalResponses > 0 ? (double)selectionCount / totalResponses * 100 : 0;

                return new OptionResultDto
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Order = option.Order,
                    SelectionCount = selectionCount,
                    Percentage = Math.Round(percentage, 2),
                    Attachment = option.Attachment != null ? new AttachmentDto(
                        option.Attachment.Id,
                        option.Attachment.FileName,
                        option.Attachment.ContentType,
                        option.Attachment.SizeBytes
                    ) : null
                };
            })
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            OptionResults = optionResults
        };
    }

    private static QuestionReportDto BuildOpenTextReport(
        Question question,
        List<Answer> answers,
        IReadOnlyList<Participation> participations,
        int totalResponses,
        double responseRate,
        bool showParticipantNames,
        Dictionary<int, SurveyInvitation>? invitationLookup)
    {
        var textResponses = answers
            .Where(a => !string.IsNullOrWhiteSpace(a.TextValue))
            .Select(a =>
            {
                var participation = participations.FirstOrDefault(p => p.Id == a.ParticipationId);
                var participantName = showParticipantNames && participation != null
                    ? GetParticipantNameForParticipation(participation, invitationLookup)
                    : null;

                return new TextResponseDto
                {
                    ParticipationId = a.ParticipationId,
                    ParticipantName = participantName,
                    TextValue = a.TextValue ?? string.Empty,
                    SubmittedAt = participation?.CompletedAt ?? participation?.StartedAt ?? TimeHelper.NowInTurkey
                };
            })
            .OrderByDescending(r => r.SubmittedAt)
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            TextResponses = textResponses
        };
    }

    private static QuestionReportDto BuildFileUploadReport(
        Question question,
        List<Answer> answers,
        IReadOnlyList<Participation> participations,
        int totalResponses,
        double responseRate,
        bool showParticipantNames,
        Dictionary<int, SurveyInvitation>? invitationLookup)
    {
        var fileResponses = answers
            .Where(a => a.Attachment != null)
            .Select(a =>
            {
                var participation = participations.FirstOrDefault(p => p.Id == a.ParticipationId);
                var participantName = showParticipantNames && participation != null
                    ? GetParticipantNameForParticipation(participation, invitationLookup)
                    : null;

                return new FileResponseDto
                {
                    AnswerId = a.Id,
                    AttachmentId = a.Attachment!.Id,
                    ParticipationId = a.ParticipationId,
                    ParticipantName = participantName,
                    FileName = a.Attachment.FileName,
                    ContentType = a.Attachment.ContentType,
                    SizeBytes = a.Attachment.SizeBytes,
                    SubmittedAt = participation?.CompletedAt ?? participation?.StartedAt ?? TimeHelper.NowInTurkey
                };
            })
            .OrderByDescending(r => r.SubmittedAt)
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            FileResponses = fileResponses
        };
    }

    private QuestionReportDto BuildConditionalReport(
        Question question,
        IReadOnlyList<Participation> participations,
        int totalResponses,
        double responseRate,
        bool showParticipantNames,
        Dictionary<int, SurveyInvitation>? invitationLookup)
    {

        var conditionalResults = question.Options
            .Where(o => o.DependentQuestions.Any())
            .OrderBy(o => o.Order)
            .Select(option =>
            {

                var participantsWhoSelectedOption = participations
                    .Where(p => p.Answers.Any(a =>
                        a.QuestionId == question.Id &&
                        a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id)))
                    .ToList();

                var childQuestionReports = option.DependentQuestions
                    .OrderBy(dq => dq.ChildQuestion.Order)
                    .Select(dq => BuildQuestionReport(dq.ChildQuestion, participantsWhoSelectedOption, showParticipantNames, invitationLookup))
                    .ToList();

                return new ConditionalBranchResultDto
                {
                    ParentOptionId = option.Id,
                    ParentOptionText = option.Text,
                    ParticipantCount = participantsWhoSelectedOption.Count,
                    ChildQuestions = childQuestionReports
                };
            })
            .ToList();

        var answers = participations
            .SelectMany(p => p.Answers)
            .Where(a => a.QuestionId == question.Id)
            .ToList();

        var optionResults = question.Options
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                var selectionCount = answers.Count(a => a.SelectedOptions.Any(so => so.QuestionOptionId == option.Id));
                var percentage = totalResponses > 0 ? (double)selectionCount / totalResponses * 100 : 0;

                return new OptionResultDto
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Order = option.Order,
                    SelectionCount = selectionCount,
                    Percentage = Math.Round(percentage, 2),
                    Attachment = option.Attachment != null ? new AttachmentDto(
                        option.Attachment.Id,
                        option.Attachment.FileName,
                        option.Attachment.ContentType,
                        option.Attachment.SizeBytes
                    ) : null
                };
            })
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            OptionResults = optionResults,
            ConditionalResults = conditionalResults
        };
    }

    private static QuestionReportDto BuildMatrixReport(
        Question question,
        List<Answer> answers,
        IReadOnlyList<Participation> participations,
        int totalResponses,
        double responseRate,
        bool showParticipantNames,
        Dictionary<int, SurveyInvitation>? invitationLookup)
    {
        var matrixRowResults = question.Options
            .OrderBy(o => o.Order)
            .Select(option =>
            {
                // Get all answer options for this matrix row
                var rowAnswers = answers
                    .SelectMany(a => a.SelectedOptions)
                    .Where(ao => ao.QuestionOptionId == option.Id && ao.ScaleValue.HasValue)
                    .ToList();

                var rowResponseCount = rowAnswers.Count;

                // Calculate scale distribution (count for each 1-5)
                var scaleDistribution = new int[5];
                foreach (var ao in rowAnswers)
                {
                    if (ao.ScaleValue.HasValue && ao.ScaleValue.Value >= 1 && ao.ScaleValue.Value <= 5)
                    {
                        scaleDistribution[ao.ScaleValue.Value - 1]++;
                    }
                }

                // Calculate average score
                var averageScore = rowResponseCount > 0
                    ? rowAnswers.Where(ao => ao.ScaleValue.HasValue).Average(ao => ao.ScaleValue!.Value)
                    : 0;

                // Get explanations
                var explanations = rowAnswers
                    .Where(ao => !string.IsNullOrWhiteSpace(ao.Explanation))
                    .Select(ao =>
                    {
                        var answer = answers.FirstOrDefault(a => a.SelectedOptions.Contains(ao));
                        var participation = answer != null
                            ? participations.FirstOrDefault(p => p.Id == answer.ParticipationId)
                            : null;
                        var participantName = showParticipantNames && participation != null
                            ? GetParticipantNameForParticipation(participation, invitationLookup)
                            : null;

                        return new MatrixRowExplanationDto
                        {
                            ParticipationId = answer?.ParticipationId ?? 0,
                            ParticipantName = participantName,
                            ScaleValue = ao.ScaleValue ?? 0,
                            Explanation = ao.Explanation ?? string.Empty,
                            SubmittedAt = participation?.CompletedAt ?? participation?.StartedAt ?? TimeHelper.NowInTurkey
                        };
                    })
                    .OrderByDescending(e => e.SubmittedAt)
                    .ToList();

                return new MatrixRowResultDto
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Order = option.Order,
                    TotalResponses = rowResponseCount,
                    AverageScore = Math.Round(averageScore, 2),
                    ScaleDistribution = scaleDistribution,
                    Explanations = explanations
                };
            })
            .ToList();

        return new QuestionReportDto
        {
            QuestionId = question.Id,
            Text = question.Text,
            Type = question.Type.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            TotalResponses = totalResponses,
            ResponseRate = Math.Round(responseRate, 2),
            Attachment = question.Attachment != null ? new AttachmentDto(
                question.Attachment.Id,
                question.Attachment.FileName,
                question.Attachment.ContentType,
                question.Attachment.SizeBytes
            ) : null,
            MatrixScaleLabels = question.GetMatrixScaleLabels(),
            MatrixResults = matrixRowResults
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
