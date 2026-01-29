using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;

namespace SurveyBackend.Application.Invitations.Commands.Cancel;

public sealed class CancelInvitationCommandHandler : ICommandHandler<CancelInvitationCommand, bool>
{
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public CancelInvitationCommandHandler(
        ISurveyInvitationRepository invitationRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _invitationRepository = invitationRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<bool> HandleAsync(CancelInvitationCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı oturumu bulunamadı.");
        }

        var invitation = await _invitationRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new InvalidOperationException("Davetiye bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(invitation.Survey.DepartmentId, cancellationToken);

        invitation.Cancel();
        await _invitationRepository.UpdateAsync(invitation, cancellationToken);

        return true;
    }
}
