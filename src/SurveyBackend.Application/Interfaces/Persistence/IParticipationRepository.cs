using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IParticipationRepository
{
    Task<Participation?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Participation?> GetBySurveyAndParticipantAsync(int surveyId, int participantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Participation>> GetBySurveyIdAsync(int surveyId, CancellationToken cancellationToken);
    Task<Participation?> GetByParticipantNameAsync(int surveyId, string participantName, CancellationToken cancellationToken);
    Task AddAsync(Participation participation, CancellationToken cancellationToken);
    Task UpdateAsync(Participation participation, CancellationToken cancellationToken);
}
