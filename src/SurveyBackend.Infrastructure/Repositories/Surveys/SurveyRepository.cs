using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Surveys;

public sealed class SurveyRepository : ISurveyRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public SurveyRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static IQueryable<Survey> IncludeFullSurveyGraph(IQueryable<Survey> query)
    {
        return query
            .Include(s => s.Attachment)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                    .ThenInclude(o => o.DependentQuestions)
                        .ThenInclude(dq => dq.ChildQuestion)
                            .ThenInclude(cq => cq.Options)
                                .ThenInclude(o => o.Attachment)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                    .ThenInclude(o => o.DependentQuestions)
                        .ThenInclude(dq => dq.ChildQuestion)
                            .ThenInclude(cq => cq.Attachment)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Attachment)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                    .ThenInclude(o => o.Attachment)
            .AsSplitQuery();
    }

    public async Task AddAsync(Survey survey, CancellationToken cancellationToken)
    {
        await _dbContext.Surveys.AddAsync(survey, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Survey>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Surveys
            .AsNoTracking()
            .OrderByDescending(s => s.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Survey>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken)
    {
        return await _dbContext.Surveys
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId)
            .OrderByDescending(s => s.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Survey?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await IncludeFullSurveyGraph(_dbContext.Surveys)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Survey?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await IncludeFullSurveyGraph(_dbContext.Surveys)
            .FirstOrDefaultAsync(s => s.Slug == slug, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
    {
        return await _dbContext.Surveys
            .AsNoTracking()
            .AnyAsync(s => s.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<SurveyStats>> GetSurveyStatsAsync(CancellationToken cancellationToken)
    {
        var data = await _dbContext.Surveys
            .AsNoTracking()
            .Select(s => new
            {
                s.Id,
                s.Title,
                s.DepartmentId,
                s.IsPublished,
                s.CreateDate,
                s.StartDate,
                s.EndDate,
                ParticipationCount = _dbContext.Participations.Count(p => p.SurveyId == s.Id)
            })
            .OrderByDescending(s => s.CreateDate)
            .ToListAsync(cancellationToken);

        return data
            .Select(s => new SurveyStats(s.Id, s.Title, s.DepartmentId, s.IsPublished, s.CreateDate, s.StartDate, s.EndDate, s.ParticipationCount))
            .ToList();
    }

    public async Task<IReadOnlyList<SurveyStats>> GetSurveyStatsByDepartmentAsync(int departmentId, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Surveys
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId)
            .Select(s => new
            {
                s.Id,
                s.Title,
                s.DepartmentId,
                s.IsPublished,
                s.CreateDate,
                s.StartDate,
                s.EndDate,
                ParticipationCount = _dbContext.Participations.Count(p => p.SurveyId == s.Id)
            })
            .OrderByDescending(s => s.CreateDate)
            .ToListAsync(cancellationToken);

        return data
            .Select(s => new SurveyStats(s.Id, s.Title, s.DepartmentId, s.IsPublished, s.CreateDate, s.StartDate, s.EndDate, s.ParticipationCount))
            .ToList();
    }

    public async Task UpdateAsync(Survey survey, CancellationToken cancellationToken)
    {
        if (_dbContext.Entry(survey).State == EntityState.Detached)
        {
            _dbContext.Surveys.Attach(survey);
        }

        var questionsCollection = _dbContext.Entry(survey).Collection(s => s.Questions);
        if (!questionsCollection.IsLoaded)
        {
            await questionsCollection.Query()
                .Include(q => q.Options)
                .AsSplitQuery()
                .LoadAsync(cancellationToken);
        }
        else
        {
            foreach (var question in survey.Questions)
            {
                var optionsCollection = _dbContext.Entry(question).Collection(q => q.Options);
                if (!optionsCollection.IsLoaded)
                {
                    await optionsCollection.LoadAsync(cancellationToken);
                }
            }
        }

        _dbContext.Surveys.Update(survey);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Survey survey, CancellationToken cancellationToken)
    {
        _dbContext.Surveys.Remove(survey);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
