using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.TextTemplates;

namespace SurveyBackend.Application.TextTemplates.Commands.Create;

public sealed class CreateTextTemplateCommandHandler : ICommandHandler<CreateTextTemplateCommand, int>
{
    private readonly ITextTemplateRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public CreateTextTemplateCommandHandler(
        ITextTemplateRepository repository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<int> HandleAsync(CreateTextTemplateCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");
        }

        var departmentId = _currentUserService.DepartmentId
            ?? throw new UnauthorizedAccessException("Kullanıcının departman bilgisi bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(departmentId, cancellationToken);

        var template = TextTemplate.Create(command.Title, command.Content, command.Type, departmentId);

        await _repository.AddAsync(template, cancellationToken);

        return template.Id;
    }
}
