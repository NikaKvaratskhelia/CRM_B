namespace CRM_B.Application.Options;

public sealed class PerformanceOptions
{
    public const string SectionName = "Performance";

    public TimeSpan SlowRequestThreshold { get; set; } = TimeSpan.FromMilliseconds(500);
}