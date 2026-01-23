using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.Commands;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class AssignRoleToUserCommandHandler : ICommandHandler<AssignRoleToUserCommand, bool>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ICurrentUserService _currentUserService;

    public AssignRoleToUserCommandHandler(
        IAuthorizationService authorizationService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ICurrentUserService currentUserService)
    {
        _authorizationService = authorizationService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> HandleAsync(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new InvalidOperationException("Kullanıcı bulunamadı.");

        await _authorizationService.EnsureRoleManagementAsync(user.DepartmentId, cancellationToken);

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken)
                   ?? throw new InvalidOperationException("Rol bulunamadı.");

        if (!_currentUserService.IsSuperAdmin && !_currentUserService.HasPermission("ManageUsers"))
        {
            var elevatesToManageUsers = role.Permissions.Any(rp =>
                rp.Permission.Name.Equals("ManageUsers", StringComparison.OrdinalIgnoreCase));

            if (elevatesToManageUsers)
            {
                throw new UnauthorizedAccessException("Departman yöneticileri 'ManageUsers' yetkisi içeren rolleri atayamaz.");
            }
        }

        await _userRoleRepository.AssignRoleAsync(user.Id, role.Id, user.DepartmentId, cancellationToken);
        return true;
    }
}
