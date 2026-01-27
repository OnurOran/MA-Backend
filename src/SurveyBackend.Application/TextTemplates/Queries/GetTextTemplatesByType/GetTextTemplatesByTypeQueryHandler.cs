using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.TextTemplates.DTOs;

namespace SurveyBackend.Application.TextTemplates.Queries.GetTextTemplatesByType;

public sealed class GetTextTemplatesByTypeQueryHandler
    : ICommandHandler<GetTextTemplatesByTypeQuery, IReadOnlyCollection<TextTemplateDto>>
{
    private readonly ITextTemplateRepository _repository;

    public GetTextTemplatesByTypeQueryHandler(ITextTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<TextTemplateDto>> HandleAsync(
        GetTextTemplatesByTypeQuery request,
        CancellationToken cancellationToken)
    {
        var templates = await _repository.GetByDepartmentAndTypeAsync(
            request.DepartmentId, request.Type, cancellationToken);

        return templates.Select(t => new TextTemplateDto(
            t.Id,
            t.Title,
            t.Content,
            t.Type,
            t.DepartmentId,
            t.CreateDate)).ToList();
    }
}
