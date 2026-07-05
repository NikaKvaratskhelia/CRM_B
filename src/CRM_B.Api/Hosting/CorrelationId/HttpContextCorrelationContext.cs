using CRM_B.Application.Abstractions.Observability;

namespace CRM_B.Api.Hosting.CorrelationId;

public sealed class HttpContextCorrelationContext(IHttpContextAccessor accessor) : ICorrelationContext
{
    public string CurrentId =>
        accessor.HttpContext?.Items.TryGetValue(CorrelationIdMiddleware.ItemsKey, out var v) == true && v is string s
            ? s
            : string.Empty;
}