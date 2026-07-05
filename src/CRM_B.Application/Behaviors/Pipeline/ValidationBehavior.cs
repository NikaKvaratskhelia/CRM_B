using CRM_B.Application.Behaviors.Internal;
using CRM_B.Domain.Kernel.Results.Errors;
using FluentValidation;
using MediatR;

namespace CRM_B.Application.Behaviors.Pipeline;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count == 0) return await next();

        var errors = failures
            .GroupBy(f => string.IsNullOrWhiteSpace(f.PropertyName) ? "General" : f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

        return ValidationFailureFactory<TResponse>.Create(ErrorResults.Validation(errors));
    }
}