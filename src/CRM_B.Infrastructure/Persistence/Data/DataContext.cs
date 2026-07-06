using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Infrastructure.Persistence.Idempotency.Entities;
using CRM_B.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Infrastructure.Persistence.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options), IDataContext
{
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();
    internal DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        modelBuilder.Entity<User>(builder =>
        {
            builder.OwnsOne(u => u.Email,
                emailBuilder => { emailBuilder.Property(e => e.Value).HasColumnName("Email").IsRequired(); });
        });

        base.OnModelCreating(modelBuilder);
    }
}