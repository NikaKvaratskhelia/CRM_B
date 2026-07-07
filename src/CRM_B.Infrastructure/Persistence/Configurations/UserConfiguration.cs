using CRM_B.Domain.Aggregates.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_B.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.OwnsOne(u => u.Lockout, lockout =>
        {
            lockout.Property(l => l.FailedAttempts).HasColumnName("LockoutFailedAttempts");
            lockout.Property(l => l.LockedUntil).HasColumnName("LockoutLockedUntil");
            lockout.Property(l => l.LastLoginAt).HasColumnName("LockoutLastLoginAt");
        });
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);

            email.HasIndex(e => e.Value).IsUnique();
        });
    }
}