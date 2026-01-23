using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Departments;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Departments;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public DepartmentRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Department?> GetByExternalIdentifierAsync(string externalIdentifier, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.ExternalIdentifier == externalIdentifier, cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken)
    {
        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return department;
    }
}
