using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Invitations.DTOs;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Invitations.Queries.GetSurveyByToken;

public sealed class GetSurveyByTokenQueryHandler : ICommandHandler<GetSurveyByTokenQuery, TokenSurveyDto>
{
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly IParticipationRepository _participationRepository;

    public GetSurveyByTokenQueryHandler(
        ISurveyInvitationRepository invitationRepository,
        IParticipationRepository participationRepository)
    {
        _invitationRepository = invitationRepository;
        _participationRepository = participationRepository;
    }

    public async Task<TokenSurveyDto> HandleAsync(GetSurveyByTokenQuery query, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByTokenAsync(query.Token, cancellationToken)
            ?? throw new InvalidOperationException("Geçersiz davetiye kodu.");

        if (invitation.Status == InvitationStatus.Cancelled)
        {
            throw new InvalidOperationException("Bu davetiye iptal edilmiştir.");
        }

        var survey = invitation.Survey;

        if (!survey.IsPublished)
        {
            throw new InvalidOperationException("Bu anket henüz yayınlanmamıştır.");
        }

        if (survey.EndDate.HasValue && survey.EndDate.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Bu anket sona ermiştir.");
        }

        bool hasParticipated = false;
        bool isCompleted = false;
        DateTime? completedAt = null;
        int? participationId = invitation.ParticipationId;

        if (invitation.ParticipationId.HasValue)
        {
            var participation = await _participationRepository.GetByIdAsync(
                invitation.ParticipationId.Value,
                cancellationToken);

            if (participation != null)
            {
                hasParticipated = true;
                isCompleted = participation.CompletedAt.HasValue;
                completedAt = participation.CompletedAt;
            }
        }

        var questions = MapQuestions(survey.Questions);
        var attachment = survey.Attachment != null
            ? new AttachmentDto(survey.Attachment.Id, survey.Attachment.FileName, survey.Attachment.ContentType, survey.Attachment.SizeBytes)
            : null;

        return new TokenSurveyDto(
            survey.Id,
            survey.Id,
            survey.Slug,
            survey.Title,
            survey.Description,
            survey.IntroText,
            survey.ConsentText,
            survey.OutroText,
            invitation.FirstName,
            invitation.LastName,
            hasParticipated,
            isCompleted,
            completedAt,
            participationId,
            questions,
            attachment);
    }

    private static IReadOnlyCollection<SurveyQuestionDetailDto> MapQuestions(IEnumerable<Question> questions)
    {
        return questions
            .OrderBy(q => q.Order)
            .Select(q => MapQuestion(q))
            .ToList();
    }

    private static SurveyQuestionDetailDto MapQuestion(Question question)
    {
        var attachment = question.Attachment != null
            ? new AttachmentDto(question.Attachment.Id, question.Attachment.FileName, question.Attachment.ContentType, question.Attachment.SizeBytes)
            : null;

        var matrixScaleLabels = question.Type == QuestionType.Matrix
            ? new List<string?>
            {
                question.MatrixScale1Label,
                question.MatrixScale2Label,
                question.MatrixScale3Label,
                question.MatrixScale4Label,
                question.MatrixScale5Label
            }.Where(l => !string.IsNullOrEmpty(l)).Cast<string>().ToList()
            : null;

        var allowedContentTypes = !string.IsNullOrEmpty(question.AllowedAttachmentContentTypes)
            ? question.AllowedAttachmentContentTypes.Split(',').Select(s => s.Trim()).ToList()
            : null;

        return new SurveyQuestionDetailDto(
            question.Id,
            question.Text,
            question.Description,
            question.Type,
            question.Order,
            question.IsRequired,
            MapOptions(question.Options),
            attachment,
            allowedContentTypes,
            matrixScaleLabels,
            question.MatrixShowExplanation,
            question.MatrixExplanationLabel);
    }

    private static IReadOnlyCollection<SurveyOptionDetailDto> MapOptions(IEnumerable<QuestionOption> options)
    {
        return options
            .OrderBy(o => o.Order)
            .Select(o =>
            {
                var attachment = o.Attachment != null
                    ? new AttachmentDto(o.Attachment.Id, o.Attachment.FileName, o.Attachment.ContentType, o.Attachment.SizeBytes)
                    : null;

                var childQuestions = o.DependentQuestions?.Any() == true
                    ? o.DependentQuestions.Select(dq => MapQuestion(dq.ChildQuestion)).ToList()
                    : null;

                return new SurveyOptionDetailDto(o.Id, o.Text, o.Order, o.Value, attachment, childQuestions);
            })
            .ToList();
    }
}
