using CRM_B.Api.Extensions.Auth;
using CRM_B.Api.Extensions.Localization;
using CRM_B.Api.Hosting.CorrelationId;
using CRM_B.Api.Hosting.ErrorHandling;
using CRM_B.Api.Hosting.Health;
using CRM_B.Api.RateLimiting;
using CRM_B.Infrastructure.Jobs;

namespace CRM_B.Api.Extensions.Core;

public static class ApplicationExtensions
{
    public static WebApplication UseCoreApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseCors();

        app.UseAppLocalization();

        app.UseExceptionHandling();

        if (!app.Environment.IsDevelopment())
            app.UseHsts();

        app.UseHttpsRedirection();

        app.UseJwtAuth();

        app.UseRateLimiter();

        app.UseJobs();

        app.MapAppHealthChecks();

        app.MapControllers();

        app.MapGraphQL("/graphql").RequireRateLimiting(RateLimitPolicies.GraphQL);

        return app;
    }
}