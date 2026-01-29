using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface ISurveyInvitationRepository
{
    Task<SurveyInvitation?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<SurveyInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task<IReadOnlyList<SurveyInvitation>> GetBySurveyIdAsync(int surveyId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SurveyInvitation>> GetPendingBySurveyIdAsync(int surveyId, CancellationToken cancellationToken);
    Task<bool> TokenExistsAsync(string token, CancellationToken cancellationToken);
    Task<SurveyInvitation?> GetByParticipationIdAsync(int participationId, CancellationToken cancellationToken);
    Task AddAsync(SurveyInvitation invitation, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<SurveyInvitation> invitations, CancellationToken cancellationToken);
    Task UpdateAsync(SurveyInvitation invitation, CancellationToken cancellationToken);
}
