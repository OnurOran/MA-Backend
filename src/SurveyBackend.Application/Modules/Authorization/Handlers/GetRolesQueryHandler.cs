using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.DTOs;
using SurveyBackend.Application.Modules.Authorization.Queries;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class GetRolesQueryHandler : ICommandHandler<GetRolesQuery, IReadOnlyCollection<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRolesQueryHandler(IRoleRepository roleRepository, ICurrentUserService currentUserService)
    {
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<RoleDto>> HandleAsync(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);

        if (!_currentUserService.IsSuperAdmin && !_currentUserService.HasPermission("ManageUsers"))
        {
            roles = roles
                .Where(role => role.Permissions.All(rp =>
                    !rp.Permission.Name.Equals("ManageUsers", StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return roles.Select(role =>
                new RoleDto(
                    role.Id,
                    role.Name,
                    role.Description,
                    role.Permissions.Select(rp => rp.Permission.Name).ToList()))
            .ToList();
    }
}
