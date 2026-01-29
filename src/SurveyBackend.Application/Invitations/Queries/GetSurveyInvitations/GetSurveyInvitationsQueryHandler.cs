using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Invitations.DTOs;

namespace SurveyBackend.Application.Invitations.Queries.GetSurveyInvitations;

public sealed class GetSurveyInvitationsQueryHandler : ICommandHandler<GetSurveyInvitationsQuery, IReadOnlyList<InvitationDto>>
{
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetSurveyInvitationsQueryHandler(
        ISurveyInvitationRepository invitationRepository,
        ISurveyRepository surveyRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _invitationRepository = invitationRepository;
        _surveyRepository = surveyRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<IReadOnlyList<InvitationDto>> HandleAsync(GetSurveyInvitationsQuery query, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı oturumu bulunamadı.");
        }

        var survey = await _surveyRepository.GetByIdAsync(query.SurveyId, cancellationToken)
            ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

        var invitations = await _invitationRepository.GetBySurveyIdAsync(query.SurveyId, cancellationToken);

        return invitations.Select(i => new InvitationDto(
            i.Id,
            i.SurveyId,
            i.Token,
            i.FirstName,
            i.LastName,
            i.Email,
            i.Phone,
            i.DeliveryMethod,
            i.Status,
            i.SentAt,
            i.ViewedAt,
            i.CompletedAt,
            i.ParticipationId,
            i.CreateDate)).ToList();
    }
}
