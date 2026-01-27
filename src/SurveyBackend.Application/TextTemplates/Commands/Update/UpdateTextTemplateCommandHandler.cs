using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Identity;

namespace SurveyBackend.Application.TextTemplates.Commands.Update;

public sealed class UpdateTextTemplateCommandHandler : ICommandHandler<UpdateTextTemplateCommand, bool>
{
    private readonly ITextTemplateRepository _repository;
    private readonly IAuthorizationService _authorizationService;

    public UpdateTextTemplateCommandHandler(
        ITextTemplateRepository repository,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> HandleAsync(UpdateTextTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Metin şablonu bulunamadı. Id: {command.Id}");

        await _authorizationService.EnsureDepartmentScopeAsync(template.DepartmentId, cancellationToken);

        template.Update(command.Title, command.Content, command.Type);

        await _repository.UpdateAsync(template, cancellationToken);

        return true;
    }
}
