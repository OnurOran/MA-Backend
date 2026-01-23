using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Authorization.Commands;
using SurveyBackend.Domain.Roles;

namespace SurveyBackend.Application.Modules.Authorization.Handlers;

public sealed class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, int>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public CreateRoleCommandHandler(
        IAuthorizationService authorizationService,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _authorizationService = authorizationService;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<int> HandleAsync(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        await _authorizationService.EnsurePermissionAsync("ManageUsers", cancellationToken);

        if (await _roleRepository.GetByNameAsync(request.Name, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Bu isimde bir rol zaten mevcut.");
        }

        var allPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
        var selectedPermissions = allPermissions
            .Where(p => request.Permissions.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (selectedPermissions.Count == 0)
        {
            throw new InvalidOperationException("En az bir geçerli permission seçilmelidir.");
        }

        var role = Role.Create(request.Name, request.Description);
        foreach (var permission in selectedPermissions)
        {
            role.Permissions.Add(new RolePermission(role.Id, permission.Id));
        }

        await _roleRepository.AddAsync(role, cancellationToken);
        return role.Id;
    }
}
