using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CRM_B.Application.Behaviors.Pipeline;

public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest req, RequestHandlerDelegate<TResponse> next, CancellationToken ct) =>
        await next();
}