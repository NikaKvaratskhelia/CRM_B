using System.Security.Cryptography;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Auth.Identifiers;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Entities;

namespace CRM_B.Domain.Aggregates.Auth.Entities;

public sealed class Verification : Entity<VerificationId>
{
    private Verification()
    {
    }

    private Verification(UserId userId, VerificationType type, DateTime expiresAt)
    {
        UserId = userId;
        Type = type;
        ExpiresAt = expiresAt;
    }

    public DateTime ExpiresAt { get; private set; }
    public VerificationType Type { get; private set; } = VerificationType.Email;
    public string Token { get; private set; } = GenerateToken();
    public string Otp { get; private set; } = GenerateOtp();

    public User User { get; private set; } = null!;
    public UserId UserId { get; private set; }

    public bool IsExpiredAt(DateTime now) => now >= ExpiresAt;

    public static Verification Create(UserId userId, VerificationType type, DateTime expiresAt) =>
        new(userId, type, expiresAt);

    private static string GenerateOtp() =>
        RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}