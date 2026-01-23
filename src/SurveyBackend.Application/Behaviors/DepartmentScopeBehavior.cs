using SurveyBackend.Application.Interfaces.Identity;

namespace SurveyBackend.Application.Behaviors;

public sealed class DepartmentScopeBehavior<TCommand, TResult> : ICommandPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly IAuthorizationService _authorizationService;

    public DepartmentScopeBehavior(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<TResult> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken,
        Func<Task<TResult>> next)
    {
        if (command is IDepartmentScopedCommand departmentScopedCommand)
        {
            await _authorizationService.EnsureDepartmentScopeAsync(departmentScopedCommand.DepartmentId, cancellationToken);
        }

        return await next();
    }
}
