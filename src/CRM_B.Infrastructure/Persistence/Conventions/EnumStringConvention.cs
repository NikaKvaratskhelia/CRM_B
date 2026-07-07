using Microsoft.EntityFrameworkCore;

namespace CRM_B.Infrastructure.Persistence.Conventions;

public static class EnumStringConvention
{
    public static void StoreEnumsAsStrings(this ModelConfigurationBuilder builder) =>
        builder.Properties<Enum>().HaveConversion<string>();
}