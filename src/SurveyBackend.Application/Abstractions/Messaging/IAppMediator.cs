namespace SurveyBackend.Application.Abstractions.Messaging;

public interface IAppMediator
{
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResult>;
}
