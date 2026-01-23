using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.DTOs;
using SurveyBackend.Application.Modules.Authorization.Queries;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class GetPermissionsQueryHandler : ICommandHandler<GetPermissionsQuery, IReadOnlyCollection<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPermissionsQueryHandler(IPermissionRepository permissionRepository, ICurrentUserService currentUserService)
    {
        _permissionRepository = permissionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<PermissionDto>> HandleAsync(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        if (!_currentUserService.IsSuperAdmin && !_currentUserService.HasPermission("ManageUsers"))
        {
            permissions = permissions
                .Where(p => !p.Name.Equals("ManageUsers", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return permissions
            .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
            .ToList();
    }
}
