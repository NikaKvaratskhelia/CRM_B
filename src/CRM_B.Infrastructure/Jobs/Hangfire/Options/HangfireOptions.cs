using System.ComponentModel.DataAnnotations;

namespace CRM_B.Infrastructure.Jobs.Hangfire.Options;

public sealed class HangfireOptions
{
    public const string SectionName = "Hangfire";

    [Range(1, 256)] public int WorkerCount { get; init; } = 4;

    [Required] public HangfireDashboardOptions Dashboard { get; init; } = new();
}