using System.Text.Json.Serialization;

namespace CRM_B.Application.Contracts.Auth;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Authenticated), nameof(Authenticated))]
[JsonDerivedType(typeof(VerificationRequired), nameof(VerificationRequired))]
public abstract record LoginOutcome
{
    public sealed record Authenticated(
        string AccessToken,
        string RefreshToken,
        DateTimeOffset RefreshTokenExpiresAt) : LoginOutcome;

    public sealed record VerificationRequired : LoginOutcome
    {
        public static readonly VerificationRequired Instance = new();
    }
}