using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.TextTemplates;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.TextTemplates;

public sealed class TextTemplateRepository : ITextTemplateRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public TextTemplateRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TextTemplate template, CancellationToken cancellationToken)
    {
        await _dbContext.TextTemplates.AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TextTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.TextTemplates
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TextTemplate>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken)
    {
        return await _dbContext.TextTemplates
            .AsNoTracking()
            .Where(t => t.DepartmentId == departmentId)
            .OrderByDescending(t => t.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TextTemplate>> GetByDepartmentAndTypeAsync(int departmentId, TextTemplateType type, CancellationToken cancellationToken)
    {
        return await _dbContext.TextTemplates
            .AsNoTracking()
            .Where(t => t.DepartmentId == departmentId && t.Type == type)
            .OrderByDescending(t => t.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(TextTemplate template, CancellationToken cancellationToken)
    {
        if (_dbContext.Entry(template).State == EntityState.Detached)
        {
            _dbContext.TextTemplates.Attach(template);
        }

        _dbContext.TextTemplates.Update(template);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
