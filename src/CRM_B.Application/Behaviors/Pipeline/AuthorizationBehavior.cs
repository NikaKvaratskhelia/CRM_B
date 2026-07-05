using CRM_B.Application.Abstractions.Identity;
using CRM_B.Application.Abstractions.Messaging.Authorization;
using CRM_B.Application.Behaviors.Internal;
using CRM_B.Domain.Kernel.Results.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Application.Behaviors.Pipeline;

public sealed class AuthorizationBehavior<TRequest, TResponse>(IServiceProvider services)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IRequireAuthorization marker) return await next();

        var currentUser = services.GetRequiredService<ICurrentUser>();

        if (!currentUser.IsAuthenticated)
            return ValidationFailureFactory<TResponse>.Create(ErrorResults.Unauthorized);

        if (string.IsNullOrWhiteSpace(marker.Policy)) return await next();

        var evaluator = services.GetRequiredService<IPolicyEvaluator>();
        var allowed = await evaluator.EvaluateAsync(marker.Policy, cancellationToken);

        if (!allowed) return ValidationFailureFactory<TResponse>.Create(ErrorResults.Forbidden);

        return await next();
    }
}