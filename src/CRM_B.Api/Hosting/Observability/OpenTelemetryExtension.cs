using System.ComponentModel.DataAnnotations;
using CRM_B.Application.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CRM_B.Api.Hosting.Observability;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddAppObservability(
        this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
    {
        var section = config.GetSection(OtelOptions.SectionName);
        var otel = section.Get<OtelOptions>() ?? new OtelOptions();

        Validator.ValidateObject(otel, new ValidationContext(otel), validateAllProperties: true);

        services.AddOptions<OtelOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(otel.ServiceName))
            .WithTracing(tracing =>
            {
                tracing.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(otel.SamplingRatio)));
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddEntityFrameworkCoreInstrumentation();

                if (!string.IsNullOrWhiteSpace(otel.Endpoint))
                    tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otel.Endpoint));

                if (env.IsDevelopment())
                    tracing.AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddRuntimeInstrumentation();

                if (!string.IsNullOrWhiteSpace(otel.Endpoint))
                    metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otel.Endpoint));

                if (env.IsDevelopment())
                    metrics.AddConsoleExporter();
            });

        return services;
    }
}