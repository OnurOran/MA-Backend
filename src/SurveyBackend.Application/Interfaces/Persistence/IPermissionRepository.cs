using System.Collections.Generic;
using SurveyBackend.Domain.Roles;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IPermissionRepository
{
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken);
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<Permission> permissions, CancellationToken cancellationToken);
}
