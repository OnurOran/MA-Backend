using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SurveyBackend.Application.Behaviors;

namespace SurveyBackend.Application.Abstractions.Messaging;

public sealed class AppMediator : IAppMediator
{
    private readonly IServiceProvider _serviceProvider;

    public AppMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResult>
    {
        var validators = _serviceProvider.GetServices<IValidator<TCommand>>();
        await ValidationBehavior.ValidateAsync(validators, command, cancellationToken);

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        var behaviors = _serviceProvider.GetServices<ICommandPipelineBehavior<TCommand, TResult>>().ToList();
        Func<Task<TResult>> handlerDelegate = () => handler.HandleAsync(command, cancellationToken);

        if (behaviors.Any())
        {
            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = handlerDelegate;
                handlerDelegate = () => behavior.HandleAsync(command, cancellationToken, next);
            }
        }

        return await handlerDelegate();
    }
}
