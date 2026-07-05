using CRM_B.Application.Abstractions.Idempotency;

namespace CRM_B.Api.Hosting.Idempotency;

public sealed class HttpIdempotencyKeyAccessor(IHttpContextAccessor accessor) : IIdempotencyKeyAccessor
{
    public const string HeaderName = "Idempotency-Key";

    public string? Current
    {
        get
        {
            var ctx = accessor.HttpContext;
            if (ctx is null) return null;

            if (!ctx.Request.Headers.TryGetValue(HeaderName, out var header)) return null;

            var value = header.ToString();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}