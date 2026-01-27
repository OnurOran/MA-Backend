using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.TextTemplates;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface ITextTemplateRepository
{
    Task AddAsync(TextTemplate template, CancellationToken cancellationToken);
    Task<TextTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TextTemplate>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TextTemplate>> GetByDepartmentAndTypeAsync(int departmentId, TextTemplateType type, CancellationToken cancellationToken);
    Task UpdateAsync(TextTemplate template, CancellationToken cancellationToken);
}
