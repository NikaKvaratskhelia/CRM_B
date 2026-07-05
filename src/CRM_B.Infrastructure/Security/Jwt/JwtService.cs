using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Domain.Aggregates.Users.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CRM_B.Infrastructure.Security.Jwt;

public sealed class JwtService(JwtOptions settings, TimeProvider time) : IJwtService
{
    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(settings.RefreshTokenDays);

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Name, user.FirstName),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
        };

        return WriteToken(claims, TimeSpan.FromDays(settings.AccessTokenDays));
    }

    public string GenerateEmailVerification(string email, string otp)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, email),
            new("otp", otp),
        };

        return WriteToken(claims, TimeSpan.FromMinutes(15));
    }

    private string WriteToken(IEnumerable<Claim> claims, TimeSpan lifetime)
    {
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var now = time.GetUtcNow().UtcDateTime;
        var token = new JwtSecurityToken(
            claims: claims,
            issuer: settings.Issuer,
            audience: settings.Audience,
            notBefore: now,
            expires: now.Add(lifetime),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}