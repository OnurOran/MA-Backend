using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IParticipantRepository
{
    Task<Participant?> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken);
    Task<Participant?> GetByLdapUsernameAsync(string username, CancellationToken cancellationToken);
    Task AddAsync(Participant participant, CancellationToken cancellationToken);
}
