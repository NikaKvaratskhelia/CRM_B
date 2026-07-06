using CRM_B.Domain.Aggregates.Auth.Enums;

namespace CRM_B.Domain.Aggregates.Auth.Policies;

public static class VerificationLifetimes
{
    public static readonly TimeSpan Email = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan Password = TimeSpan.FromMinutes(30);

    public static TimeSpan Of(VerificationType type) => type switch
    {
        VerificationType.Email => Email,
        VerificationType.Password => Password,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported verification type."),
    };
}