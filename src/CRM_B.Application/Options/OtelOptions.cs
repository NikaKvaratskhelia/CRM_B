using System.ComponentModel.DataAnnotations;

namespace CRM_B.Application.Options;

public sealed class OtelOptions
{
    public const string SectionName = "Otel";

    [Required] public string ServiceName { get; init; } = "vs-arc-api";

    public string? Endpoint { get; init; }

    [Range(0.0, 1.0)] public double SamplingRatio { get; init; } = 1.0;
}