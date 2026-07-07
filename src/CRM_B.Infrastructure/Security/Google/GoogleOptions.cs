using System.ComponentModel.DataAnnotations;

namespace CRM_B.Infrastructure.Security.Google;

public sealed class GoogleOptions
{
    public const string SectionName = "Authentication:Google";

    [Required] public string ClientId { get; init; } = string.Empty;
}