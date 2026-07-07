using CRM_B.Infrastructure.Persistence.Idempotency.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_B.Infrastructure.Persistence.Configurations;

public class IdempotencyKeyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.HasKey(ik => ik.Key);

        builder.Property(ik => ik.Key)
            .HasMaxLength(450);

        builder.Property(ik => ik.RequestHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(ik => ik.ResponsePayload)
            .IsRequired();
    }
}