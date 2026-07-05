using System.ComponentModel.DataAnnotations;

namespace CRM_B.Application.Options;

public sealed class IdempotencyOptions
{
    public const string SectionName = "Idempotency";

    [Required] public TimeSpan Ttl { get; set; } = TimeSpan.FromHours(24);

    [Required] public string PruneCron { get; set; } = "0 * * * *";
}