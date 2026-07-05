using System.Security.Cryptography;
using System.Text.Json;
using CRM_B.Application.Abstractions.Idempotency;
using CRM_B.Application.Behaviors.Internal;
using CRM_B.Application.Options;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM_B.Application.Behaviors.Pipeline;

public sealed class IdempotencyBehavior<TRequest, TResponse>(
    IIdempotencyKeyAccessor keys,
    IIdempotencyStore store,
    IOptions<IdempotencyOptions> options,
    TimeProvider time,
    ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IdempotencyMarker<TRequest>.IsIdempotent) return await next();

        var key = keys.Current;
        if (string.IsNullOrWhiteSpace(key)) return await next();

        var hash = HashCommand(request);
        var existing = await store.FindAsync(key, cancellationToken);

        if (existing is not null)
        {
            if (existing.RequestHash == hash)
            {
                logger.LogInformation(
                    "Idempotency hit for {RequestName} (key {IdempotencyKey})",
                    typeof(TRequest).Name, key);
            }

            logger.LogWarning(
                "Idempotency hash mismatch for {RequestName} (key {IdempotencyKey}) — returning conflict",
                typeof(TRequest).Name, key);
            return IdempotencyResponseFactory<TResponse>.Failure(ErrorResults.IdempotencyConflict);
        }

        var response = await next();

        if (response is Result result && result.IsSuccess)
        {
            var payload = IdempotencyResponseFactory<TResponse>.Serialize(response);
            var expiresAt = time.GetUtcNow().UtcDateTime.Add(options.Value.Ttl);
            store.Stage(key, hash, payload, expiresAt);
        }

        return response;
    }

    private static string HashCommand(TRequest request)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(request, typeof(TRequest));
        return Convert.ToBase64String(SHA256.HashData(json));
    }
}