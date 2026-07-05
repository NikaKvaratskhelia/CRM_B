using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM_B.Api.Hosting.ErrorHandling;

public sealed class ResultFilter(ProblemDetailsFactory problemDetails) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not ObjectResult { Value: Result result }) return;

        if (!result.IsSuccess && result.Error is { } error)
        {
            context.Result = BuildProblem(context.HttpContext, error);
            return;
        }

        context.Result = result.HasValue
            ? new OkObjectResult(result.GetValue())
            : new NoContentResult();
    }

    private ObjectResult BuildProblem(HttpContext ctx, ErrorResults error)
    {
        var details = problemDetails.CreateFromError(error, ctx);
        return new ObjectResult(details)
        {
            StatusCode = details.Status,
            ContentTypes = { "application/problem+json" },
        };
    }
}