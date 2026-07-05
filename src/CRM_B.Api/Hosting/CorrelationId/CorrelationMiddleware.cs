using System.Diagnostics;

namespace CRM_B.Api.Hosting.CorrelationId;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";
    public const string ItemsKey = "CorrelationId";

    public async Task InvokeAsync(HttpContext ctx)
    {
        var correlationId = ReadOrCreate(ctx);

        ctx.Items[ItemsKey] = correlationId;
        ctx.Response.Headers[HeaderName] = correlationId;
        Activity.Current?.SetTag("correlation.id", correlationId);

        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            [ItemsKey] = correlationId,
        });

        await next(ctx);
    }

    private static string ReadOrCreate(HttpContext ctx)
    {
        if (ctx.Request.Headers.TryGetValue(HeaderName, out var header))
        {
            var value = header.ToString();
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }

        return Guid.NewGuid().ToString();
    }
}