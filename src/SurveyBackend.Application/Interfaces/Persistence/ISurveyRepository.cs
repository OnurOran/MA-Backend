using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface ISurveyRepository
{
    Task AddAsync(Survey survey, CancellationToken cancellationToken);
    Task<IReadOnlyList<Survey>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Survey>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken);
    Task<Survey?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Survey?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken);
    Task<IReadOnlyList<SurveyStats>> GetSurveyStatsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SurveyStats>> GetSurveyStatsByDepartmentAsync(int departmentId, CancellationToken cancellationToken);
    Task UpdateAsync(Survey survey, CancellationToken cancellationToken);
    Task DeleteAsync(Survey survey, CancellationToken cancellationToken);
}
