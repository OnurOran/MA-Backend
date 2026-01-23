using System.Collections.Generic;
using SurveyBackend.Domain.Roles;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IRoleRepository
{
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Role role, CancellationToken cancellationToken);
    Task UpdateAsync(Role role, CancellationToken cancellationToken);
}
