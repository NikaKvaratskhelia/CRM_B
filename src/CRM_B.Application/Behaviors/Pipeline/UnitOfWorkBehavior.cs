using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Behaviors.Internal;
using CRM_B.Domain.Kernel.Results;
using MediatR;

namespace CRM_B.Application.Behaviors.Pipeline;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork uow)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest req, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next();

        if (!CommandMarker<TRequest>.IsCommand) return response;

        if (response is Result result && result.IsSuccess)
            await uow.SaveChangesAsync(ct);

        return response;
    }
}