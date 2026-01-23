using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Roles;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Authorization;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public PermissionRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Permissions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Permission> permissions, CancellationToken cancellationToken)
    {
        await _dbContext.Permissions.AddRangeAsync(permissions, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
