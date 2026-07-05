using CRM_B.Application.Abstractions.Localization;
using CRM_B.Domain.Kernel.Results.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CRM_B.Api.Hosting.ErrorHandling;

public sealed class ProblemDetailsFactory(IErrorLocalizer localizer)
{
    private const string ProblemTypePrefix = "https://httpstatuses.io/";

    public ProblemDetails CreateFromError(ErrorResults error, HttpContext ctx)
    {
        var status = StatusFromKind(error.Kind);

        var details = new ProblemDetails
        {
            Status = status,
            Instance = ctx.Request.Path,
            Type = ProblemTypePrefix + status,
            Title = localizer.Get(error.Code, error.Args),
        };

        AttachExtensions(details, error, ctx);
        return details;
    }

    public ProblemDetails CreateInternal(HttpContext ctx)
    {
        const int status = StatusCodes.Status500InternalServerError;

        var details = new ProblemDetails
        {
            Status = status,
            Instance = ctx.Request.Path,
            Type = ProblemTypePrefix + status,
            Title = localizer[ErrorResults.InternalError.Code],
        };

        AttachExtensions(details, ErrorResults.InternalError, ctx);
        return details;
    }

    private static int StatusFromKind(ErrorKind kind) => kind switch
    {
        ErrorKind.Validation => StatusCodes.Status400BadRequest,
        ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        ErrorKind.Conflict => StatusCodes.Status409Conflict,
        ErrorKind.Internal => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError,
    };

    private static void AttachExtensions(ProblemDetails details, ErrorResults error, HttpContext ctx)
    {
        details.Extensions["code"] = error.Code;
        details.Extensions["kind"] = KindString(error.Kind);

        if (error.Args is { Length: > 0 })
            details.Extensions["args"] = error.Args;

        if (error.Errors is not null)
            details.Extensions["errors"] = error.Errors;

        details.Extensions["traceId"] = ctx.TraceIdentifier;

        if (ctx.Items.TryGetValue("CorrelationId", out var cid) && cid is string s)
            details.Extensions["correlationId"] = s;
    }

    private static string KindString(ErrorKind kind) => kind switch
    {
        ErrorKind.Validation => "validation",
        ErrorKind.Unauthorized => "unauthorized",
        ErrorKind.Forbidden => "forbidden",
        ErrorKind.NotFound => "notfound",
        ErrorKind.Conflict => "conflict",
        ErrorKind.Internal => "internal",
        _ => "internal",
    };
}