using System.ComponentModel.DataAnnotations;

namespace CRM_B.Infrastructure.Jobs.Hangfire.Options;

public sealed class HangfireDashboardOptions
{
    public bool Enabled { get; init; } = true;

    [Required] public string Path { get; init; } = "/hangfire";

    [Required] public string User { get; init; } = "admin";

    [Required] public string Password { get; init; } = string.Empty;
}