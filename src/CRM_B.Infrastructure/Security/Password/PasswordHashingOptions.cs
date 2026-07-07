using System.ComponentModel.DataAnnotations;

namespace VS_ARC.Infrastructure.Security.Password;

public sealed class PasswordHashingOptions
{
    public const string SectionName = "PasswordHashing";

    [Range(4, 16)] public int WorkFactor { get; init; } = 12;
}