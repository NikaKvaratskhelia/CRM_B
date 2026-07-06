using System.ComponentModel.DataAnnotations;

namespace CRM_B.Infrastructure.Jobs.Outbox.Options;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    [Required] public string SafetyNetCron { get; init; } = "*/5 * * * *";

    [Required] public string CleanupCron { get; init; } = "0 3 * * *";

    public TimeSpan StragglerThreshold { get; init; } = TimeSpan.FromMinutes(2);

    public TimeSpan RetentionPeriod { get; init; } = TimeSpan.FromDays(30);

    [Range(1, 10_000)] public int BatchSize { get; init; } = 100;

    [Range(1, 100)] public int MaxRetries { get; init; } = 5;
}