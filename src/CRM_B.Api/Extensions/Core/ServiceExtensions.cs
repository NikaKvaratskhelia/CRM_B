using System.Reflection;
using Asp.Versioning;
using CRM_B.Api.Extensions.Auth;
using CRM_B.Api.Extensions.GraphQl;
using CRM_B.Api.Hosting.CorrelationId;
using CRM_B.Api.Hosting.ErrorHandling;
using CRM_B.Api.Hosting.Health;
using CRM_B.Api.Hosting.Idempotency;
using CRM_B.Api.Hosting.Identity;
using CRM_B.Api.Hosting.Observability;
using CRM_B.Api.RateLimiting;
using CRM_B.Application;
using CRM_B.Application.Abstractions.Idempotency;
using CRM_B.Application.Abstractions.Identity;
using CRM_B.Application.Abstractions.Observability;
using CRM_B.Application.Options;
using CRM_B.Infrastructure;
using Path = System.IO.Path;

namespace CRM_B.Api.Extensions.Core;

public static class ServicesExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection service, IConfiguration config,
        IWebHostEnvironment env)
    {
        service.AddApplication();
        service.AddInfrastructure(config);

        service.AddOptions<PerformanceOptions>()
            .Bind(config.GetSection(PerformanceOptions.SectionName));

        service.AddSingleton<ProblemDetailsFactory>();
        service.AddControllers(o => o.Filters.Add<ResultFilter>());
        service.AddGraphqlConfiguration(env);
        service.AddHttpContextAccessor();
        service.AddScoped<ICurrentUser, CurrentUser>();
        service.AddScoped<IPolicyEvaluator, PolicyEvaluator>();
        service.AddScoped<ICorrelationContext, HttpContextCorrelationContext>();
        service.AddScoped<IIdempotencyKeyAccessor, HttpIdempotencyKeyAccessor>();
        service.AddJwtAuth();
        service.AddAppRateLimiting();
        service.AddServerCors(config);
        service.AddApiVersioningConfiguration();
        service.AddAppObservability(config, env);
        service.AddAppHealthChecks();
        service.AddSwaggerXml();

        return service;
    }

    private static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = false;
                o.ReportApiVersions = true;
            })
            .AddMvc();

        return services;
    }

    private static IServiceCollection AddServerCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>()!;

        services.AddCors(cors =>
        {
            cors.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("X-Correlation-Id")
                    .SetPreflightMaxAge(TimeSpan.FromHours(24));
            });
        });

        return services;
    }

    private static IServiceCollection AddSwaggerXml(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            var xml = Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            if (File.Exists(xml)) o.IncludeXmlComments(xml);
        });

        return services;
    }
}