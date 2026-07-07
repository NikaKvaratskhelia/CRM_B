using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Aggregates.Auth.Entities;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Kernel.Entities;
using CRM_B.Infrastructure.Persistence.Conventions;
using CRM_B.Infrastructure.Persistence.Idempotency.Entities;
using CRM_B.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Infrastructure.Persistence.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options), IDataContext
{
    // Outbox (infrastructure-only — not exposed via IDataContext)
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();
    internal DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Verification> Verifications => Set<Verification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        modelBuilder.IgnoreDomainEvents();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.RegisterStronglyTypedIds(typeof(IEntityId<>).Assembly);
        configurationBuilder.StoreEnumsAsStrings();
        configurationBuilder.StoreDatesAsUtc();
    }
}