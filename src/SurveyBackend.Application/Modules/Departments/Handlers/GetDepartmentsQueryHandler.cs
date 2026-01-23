using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Departments.DTOs;
using SurveyBackend.Application.Modules.Departments.Queries;

namespace SurveyBackend.Application.Modules.Departments.Handlers;

public sealed class GetDepartmentsQueryHandler : ICommandHandler<GetDepartmentsQuery, IReadOnlyCollection<DepartmentDto>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentsQueryHandler(
        IAuthorizationService authorizationService,
        IDepartmentRepository departmentRepository)
    {
        _authorizationService = authorizationService;
        _departmentRepository = departmentRepository;
    }

    public async Task<IReadOnlyCollection<DepartmentDto>> HandleAsync(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        await _authorizationService.EnsurePermissionAsync("ManageUsers", cancellationToken);

        var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        return departments
            .Select(d => new DepartmentDto(d.Id, d.Name, d.ExternalIdentifier))
            .ToList();
    }
}
