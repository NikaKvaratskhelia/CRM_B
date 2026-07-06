using System.Security.Cryptography;
using CRM_B.Domain.Aggregates.Auth.Identifiers;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Entities;

namespace CRM_B.Domain.Aggregates.Auth.Entities;

public sealed class RefreshToken : Entity<RefreshTokenId>
{
    private RefreshToken()
    {
    }

    public string Token { get; private set; } = GenerateToken();
    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public User User { get; private set; } = null!;
    public UserId UserId { get; private set; }

    public bool IsRevoked => RevokedAt is not null;

    public bool IsExpiredAt(DateTime now) => now >= ExpiresAt;

    public static RefreshToken Create(UserId userId, DateTime expiresAt) => new()
    {
        UserId = userId,
        ExpiresAt = expiresAt,
    };

    public void Revoke(DateTime now) => RevokedAt ??= now;

    public void RevokeAndReplace(string newToken, DateTime now)
    {
        Revoke(now);
        ReplacedByToken = newToken;
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}