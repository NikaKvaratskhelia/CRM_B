using System.ComponentModel.DataAnnotations;

namespace CRM_B.Infrastructure.Security.Jwt;

public sealed class JwtOptions : IValidatableObject
{
    public const string SectionName = "Jwt";

    [Required] public string Issuer { get; init; } = string.Empty;

    [Required] public string Audience { get; init; } = string.Empty;

    [Required, MinLength(32)] public string SigningKey { get; init; } = string.Empty;

    [Range(1, 365)] public int AccessTokenDays { get; init; } = 7;

    [Range(1, 365)] public int RefreshTokenDays { get; init; } = 30;

    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        if (SigningKey.StartsWith("REPLACE_WITH", StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult(
                "Jwt:SigningKey is set to a placeholder. Replace it with a real secret before running.",
                [nameof(SigningKey)]);
    }
}