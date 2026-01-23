using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Modules.Departments.DTOs;
using SurveyBackend.Application.Modules.Departments.Queries;

namespace SurveyBackend.Application.Modules.Departments.Handlers;

public sealed class GetDepartmentUsersQueryHandler : ICommandHandler<GetDepartmentUsersQuery, IReadOnlyCollection<DepartmentUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentUsersQueryHandler(IUserRepository userRepository, IDepartmentRepository departmentRepository)
    {
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<IReadOnlyCollection<DepartmentUserDto>> HandleAsync(GetDepartmentUsersQuery request, CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
        if (department is null)
        {
            throw new InvalidOperationException("Departman bulunamadı.");
        }

        var users = await _userRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

        return users
            .Select(user =>
            {
                var roles = user.Roles
                    .Where(r => r.DepartmentId == request.DepartmentId)
                    .Select(r => r.Role.Name)
                    .Distinct()
                    .ToList();

                return new DepartmentUserDto(user.Id, user.Username, user.Email, roles);
            })
            .ToList();
    }
}
