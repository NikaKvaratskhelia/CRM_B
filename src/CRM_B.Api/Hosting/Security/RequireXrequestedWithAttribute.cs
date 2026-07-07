using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM_B.Api.Hosting.Security;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class RequireXRequestedWithAttribute : Attribute, IActionFilter
{
    private const string HeaderName = "X-Requested-With";
    private const string ExpectedValue = "XMLHttpRequest";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var header = context.HttpContext.Request.Headers[HeaderName].ToString();

        if (!string.Equals(header, ExpectedValue, StringComparison.OrdinalIgnoreCase))
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}