using System.Net.Http.Headers;
using System.Text;
using CRM_B.Infrastructure.Jobs.Hangfire.Options;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace CRM_B.Infrastructure.Jobs.Hangfire.Dashboard;

public sealed class HangfireDashboardAuthFilter(HangfireDashboardOptions options) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        var header = http.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(header) && AuthenticationHeaderValue.TryParse(header, out var auth)
                                          && auth.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase)
                                          && auth.Parameter is { Length: > 0 } encoded)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded)).Split(':', 2);

            if (decoded.Length == 2 && decoded[0] == options.User && decoded[1] == options.Password) return true;
        }

        http.Response.StatusCode = StatusCodes.Status401Unauthorized;
        http.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire\"";

        return false;
    }
}