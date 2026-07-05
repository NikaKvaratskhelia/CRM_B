using System.Diagnostics;
using CRM_B.Application.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM_B.Application.Behaviors.Pipeline;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    IOptions<PerformanceOptions> options)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        var threshold = options.Value.SlowRequestThreshold;
        if (stopwatch.Elapsed > threshold)
        {
            logger.LogWarning(
                "Slow request: {RequestName} took {Elapsed}ms (threshold {Threshold}ms)",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds,
                (long)threshold.TotalMilliseconds);
        }

        return response;
    }
}