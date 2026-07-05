using System.Diagnostics;
using CRM_B.Domain.Kernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM_B.Application.Behaviors.Pipeline;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;

        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = name
        });

        logger.LogInformation("Handling {RequestName}", name);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            if (response is Result result && result.IsFailure)
            {
                var error = result.Error!;
                if (error.Errors is not null)
                {
                    logger.LogInformation(
                        "Validation failed for {RequestName} in {Elapsed}ms ({Code})",
                        name, stopwatch.ElapsedMilliseconds, error.Code);
                }
                else
                {
                    logger.LogInformation(
                        "{RequestName} failed in {Elapsed}ms ({Code}, {Kind})",
                        name, stopwatch.ElapsedMilliseconds, error.Code, error.Kind);
                }
            }
            else
            {
                logger.LogInformation(
                    "{RequestName} succeeded in {Elapsed}ms",
                    name, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            logger.LogInformation(
                "{RequestName} cancelled after {Elapsed}ms",
                name, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex,
                "{RequestName} threw {ExceptionType} after {Elapsed}ms",
                name, ex.GetType().Name, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}