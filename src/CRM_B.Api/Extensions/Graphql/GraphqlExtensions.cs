using CRM_B.Api.Extensions.Graphql;

namespace CRM_B.Api.Extensions.GraphQl;

public static class GraphqlExtensions
{
    public static IServiceCollection AddGraphqlConfiguration(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddGraphQLServer()
            .AddMaxExecutionDepthRule(7, skipIntrospectionFields: true)
            .DisableIntrospection(env.IsProduction())
            .ModifyRequestOptions(o =>
            {
                o.IncludeExceptionDetails = env.IsDevelopment();
                o.ExecutionTimeout = TimeSpan.FromSeconds(15);
            })
            .ModifyPagingOptions(o =>
            {
                o.IncludeTotalCount = true;
                o.DefaultPageSize = 10;
                o.MaxPageSize = 30;
            })
            .ModifyCostOptions(o =>
            {
                o.MaxTypeCost = 5_000;
                o.MaxFieldCost = 75_000;
                o.DefaultResolverCost = 0;
                o.ApplyCostDefaults = true;
            })
            .AddCostAnalyzer()
            .SetMaxAllowedValidationErrors(4)
            .AddGraphqlQueries()
            .AddGraphqlTypes()
            .AddGraphqlFilters()
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting();

        return services;
    }
}