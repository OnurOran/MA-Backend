using System;
using System.Collections.Concurrent;
using System.Linq;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Departments;

namespace SurveyBackend.Infrastructure.Repositories.Departments;

public sealed class InMemoryDepartmentRepository : IDepartmentRepository
{
    private static readonly ConcurrentDictionary<string, Department> _departments = new(StringComparer.OrdinalIgnoreCase);

    static InMemoryDepartmentRepository()
    {

        _departments.TryAdd("hr", Department.Create("Human Resources", "hr"));
        _departments.TryAdd("engineering", Department.Create("Engineering", "engineering"));
        _departments.TryAdd("sales", Department.Create("Sales", "sales"));
    }

    public Task<Department?> GetByExternalIdentifierAsync(string externalIdentifier, CancellationToken cancellationToken)
    {
        _departments.TryGetValue(externalIdentifier, out var department);
        return Task.FromResult(department);
    }

    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var department = _departments.Values.FirstOrDefault(d => d.Id == id);
        return Task.FromResult(department);
    }

    public Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Department> departments = _departments.Values.ToList();
        return Task.FromResult(departments);
    }

    public Task<Department> AddAsync(Department department, CancellationToken cancellationToken)
    {
        _departments.TryAdd(department.ExternalIdentifier, department);
        return Task.FromResult(department);
    }
}
