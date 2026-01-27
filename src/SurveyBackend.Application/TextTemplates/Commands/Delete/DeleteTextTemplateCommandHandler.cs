using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Identity;

namespace SurveyBackend.Application.TextTemplates.Commands.Delete;

public sealed class DeleteTextTemplateCommandHandler : ICommandHandler<DeleteTextTemplateCommand, bool>
{
    private readonly ITextTemplateRepository _repository;
    private readonly IAuthorizationService _authorizationService;

    public DeleteTextTemplateCommandHandler(
        ITextTemplateRepository repository,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> HandleAsync(DeleteTextTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Metin şablonu bulunamadı. Id: {command.Id}");

        await _authorizationService.EnsureDepartmentScopeAsync(template.DepartmentId, cancellationToken);

        template.Delete();

        await _repository.UpdateAsync(template, cancellationToken);

        return true;
    }
}
