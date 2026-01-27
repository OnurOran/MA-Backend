using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.TextTemplates.DTOs;

namespace SurveyBackend.Application.TextTemplates.Queries.GetTextTemplates;

public sealed class GetTextTemplatesQueryHandler
    : ICommandHandler<GetTextTemplatesQuery, IReadOnlyCollection<TextTemplateDto>>
{
    private readonly ITextTemplateRepository _repository;

    public GetTextTemplatesQueryHandler(ITextTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<TextTemplateDto>> HandleAsync(
        GetTextTemplatesQuery request,
        CancellationToken cancellationToken)
    {
        var templates = await _repository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

        return templates.Select(t => new TextTemplateDto(
            t.Id,
            t.Title,
            t.Content,
            t.Type,
            t.DepartmentId,
            t.CreateDate)).ToList();
    }
}
