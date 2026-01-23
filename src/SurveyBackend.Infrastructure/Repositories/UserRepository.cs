using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Users;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public UserRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetLocalByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsSuperAdmin, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.DepartmentId == departmentId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {

        _dbContext.ChangeTracker.DetectChanges();

        var modifiedEntities = _dbContext.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in modifiedEntities)
        {
            if (entry.Entity is RefreshToken token)
            {
                var exists = await _dbContext.RefreshTokens.AnyAsync(t => t.Id == token.Id, cancellationToken);
                if (!exists)
                    entry.State = EntityState.Added;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
