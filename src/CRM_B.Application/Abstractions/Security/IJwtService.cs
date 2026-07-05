using CRM_B.Domain.Aggregates.Users.Entities;

namespace CRM_B.Application.Abstractions.Security;

public interface IJwtService
{
    TimeSpan RefreshTokenLifetime { get; }

    string GenerateAccessToken(User user);
    string GenerateEmailVerification(string email, string otp);
}