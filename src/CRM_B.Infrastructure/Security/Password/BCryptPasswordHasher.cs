using CRM_B.Application.Abstractions.Security;
using VS_ARC.Infrastructure.Security.Password;

namespace CRM_B.Infrastructure.Security.Password;

public sealed class BCryptPasswordHasher(PasswordHashingOptions options) : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, options.WorkFactor);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}