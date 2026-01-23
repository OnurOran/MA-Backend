using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Participations;

public sealed class ParticipantRepository : IParticipantRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public ParticipantRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Participant?> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken)
    {
        return await _dbContext.Participants
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ExternalId == externalId, cancellationToken);
    }

    public async Task<Participant?> GetByLdapUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _dbContext.Participants
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.LdapUsername == username, cancellationToken);
    }

    public async Task AddAsync(Participant participant, CancellationToken cancellationToken)
    {
        await _dbContext.Participants.AddAsync(participant, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
