namespace SurveyBackend.Application.Abstractions.Messaging;

public interface ICommandPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken,
        Func<Task<TResult>> next);
}
