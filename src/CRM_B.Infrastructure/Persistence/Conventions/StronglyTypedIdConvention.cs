using System.Linq.Expressions;
using System.Reflection;
using CRM_B.Domain.Kernel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CRM_B.Infrastructure.Persistence.Conventions;

public static class StronglyTypedIdConvention
{
    public static void RegisterStronglyTypedIds(this ModelConfigurationBuilder builder, Assembly domainAssembly)
    {
        var idTypes = domainAssembly.GetTypes()
            .Where(t => t.IsValueType && !t.IsAbstract && ImplementsIEntityId(t));

        foreach (var idType in idTypes)
        {
            var converterType = typeof(StronglyTypedIdConverter<>).MakeGenericType(idType);
            builder.Properties(idType).HaveConversion(converterType);
        }
    }

    private static bool ImplementsIEntityId(Type type) =>
        type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityId<>));
}

public sealed class StronglyTypedIdConverter<TId> : ValueConverter<TId, Guid>
    where TId : struct, IEntityId<TId>
{
    public StronglyTypedIdConverter() : base(id => id.Value, BuildFromGuid())
    {
    }

    private static Expression<Func<Guid, TId>> BuildFromGuid()
    {
        var ctor = typeof(TId).GetConstructor([typeof(Guid)])
                   ?? throw new InvalidOperationException(
                       $"Strongly-typed id {typeof(TId).Name} must have a constructor taking Guid.");

        var param = Expression.Parameter(typeof(Guid), "value");
        return Expression.Lambda<Func<Guid, TId>>(Expression.New(ctor, param), param);
    }
}