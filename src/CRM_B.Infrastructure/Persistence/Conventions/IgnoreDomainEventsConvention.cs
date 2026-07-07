using CRM_B.Domain.Kernel.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Infrastructure.Persistence.Conventions;

public static class IgnoreDomainEventsConvention
{
    public static void IgnoreDomainEvents(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAggregateRoot).IsAssignableFrom(entity.ClrType))
                modelBuilder.Entity(entity.ClrType).Ignore(nameof(IAggregateRoot.Events));
        }
    }
}