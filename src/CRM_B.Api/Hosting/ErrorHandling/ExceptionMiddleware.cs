namespace CRM_B.Api.Hosting.ErrorHandling;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private const int StatusClientClosedRequest = 499;

    public async Task Invoke(HttpContext ctx, ProblemDetailsFactory factory)
    {
        try
        {
            await next(ctx);
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
            HandleCancellation(ctx);
        }
        catch (Exception ex)
        {
            await HandleUnhandledAsync(ctx, ex, factory);
        }
    }

    private void HandleCancellation(HttpContext ctx)
    {
        logger.LogInformation(
            "Request cancelled at {Method} {Path} (TraceId: {TraceId})",
            ctx.Request.Method, ctx.Request.Path, ctx.TraceIdentifier);

        if (!ctx.Response.HasStarted)
            ctx.Response.StatusCode = StatusClientClosedRequest;
    }

    private async Task HandleUnhandledAsync(HttpContext ctx, Exception ex, ProblemDetailsFactory factory)
    {
        logger.LogError(ex,
            "Unhandled exception at {Method} {Path} (TraceId: {TraceId})",
            ctx.Request.Method, ctx.Request.Path, ctx.TraceIdentifier);

        var problem = factory.CreateInternal(ctx);
        await WriteAsync(ctx, problem.Status!.Value, problem);
    }

    private static async Task WriteAsync(HttpContext ctx, int status, object body)
    {
        if (ctx.Response.HasStarted) return;

        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";
        await ctx.Response.WriteAsJsonAsync(body);
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}