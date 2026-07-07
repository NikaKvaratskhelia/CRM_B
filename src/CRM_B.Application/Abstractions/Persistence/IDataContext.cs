using CRM_B.Domain.Aggregates.Auth.Entities;
using CRM_B.Domain.Aggregates.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Abstractions.Persistence;

public interface IDataContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Verification> Verifications { get; }
}