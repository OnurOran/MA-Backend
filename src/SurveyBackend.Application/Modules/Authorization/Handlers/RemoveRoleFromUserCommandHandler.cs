using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class RemoveRoleFromUserCommandHandler : ICommandHandler<RemoveRoleFromUserCommand, bool>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public RemoveRoleFromUserCommandHandler(
        IAuthorizationService authorizationService,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _authorizationService = authorizationService;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<bool> HandleAsync(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new InvalidOperationException("Kullanıcı bulunamadı.");

        await _authorizationService.EnsureRoleManagementAsync(user.DepartmentId, cancellationToken);

        await _userRoleRepository.RemoveRoleAsync(user.Id, request.RoleId, user.DepartmentId, cancellationToken);
        return true;
    }
}
