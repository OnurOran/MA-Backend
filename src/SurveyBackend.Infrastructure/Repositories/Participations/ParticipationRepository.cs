using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Participations;

public sealed class ParticipationRepository : IParticipationRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public ParticipationRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Participation?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Participations
            .Include(p => p.Participant)
            .Include(p => p.Answers)
                .ThenInclude(a => a.SelectedOptions)
                    .ThenInclude(so => so.QuestionOption)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Attachment)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Participation?> GetBySurveyAndParticipantAsync(int surveyId, int participantId, CancellationToken cancellationToken)
    {
        return await _dbContext.Participations
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.SurveyId == surveyId && p.ParticipantId == participantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Participation>> GetBySurveyIdAsync(int surveyId, CancellationToken cancellationToken)
    {
        return await _dbContext.Participations
            .Include(p => p.Participant)
            .Include(p => p.Answers)
                .ThenInclude(a => a.SelectedOptions)
                    .ThenInclude(so => so.QuestionOption)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Attachment)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Question)
            .Where(p => p.SurveyId == surveyId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Participation?> GetByParticipantNameAsync(int surveyId, string participantName, CancellationToken cancellationToken)
    {
        return await _dbContext.Participations
            .Include(p => p.Participant)
            .Include(p => p.Answers)
                .ThenInclude(a => a.SelectedOptions)
                    .ThenInclude(so => so.QuestionOption)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Attachment)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Question)
            .Where(p => p.SurveyId == surveyId &&
                       p.Participant.LdapUsername != null &&
                       p.Participant.LdapUsername.ToLower().Contains(participantName.ToLower()))
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Participation participation, CancellationToken cancellationToken)
    {
        await _dbContext.Participations.AddAsync(participation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Participation participation, CancellationToken cancellationToken)
    {

        _dbContext.ChangeTracker.DetectChanges();

        var modifiedEntities = _dbContext.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in modifiedEntities)
        {
            if (entry.Entity is Answer answer)
            {
                var exists = await _dbContext.Answers.AnyAsync(a => a.Id == answer.Id, cancellationToken);
                if (!exists)
                    entry.State = EntityState.Added;
            }
            else if (entry.Entity is AnswerOption option)
            {
                var exists = await _dbContext.AnswerOptions.AnyAsync(ao => ao.Id == option.Id, cancellationToken);
                if (!exists)
                    entry.State = EntityState.Added;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
