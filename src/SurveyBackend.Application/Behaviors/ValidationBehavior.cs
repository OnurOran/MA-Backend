using System.Linq;
using FluentValidation;

namespace SurveyBackend.Application.Behaviors;

public static class ValidationBehavior
{
    public static async Task ValidateAsync<TRequest>(IEnumerable<IValidator<TRequest>> validators, TRequest request, CancellationToken cancellationToken)
    {
        if (validators is null)
        {
            return;
        }

        var validatorList = validators.ToList();
        if (!validatorList.Any())
        {
            return;
        }

        var context = new ValidationContext<TRequest>(request);
        var validationTasks = validatorList.Select(v => v.ValidateAsync(context, cancellationToken));
        var validationResults = await Task.WhenAll(validationTasks);
        var failures = validationResults.SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }
    }
}
