using CRM_B.Api.Presentation.GraphQL.Users.Filters;
using HotChocolate.Execution.Configuration;

namespace CRM_B.Api.Extensions.Graphql;

public static class GraphqlFilterExtensions
{
    public static IRequestExecutorBuilder AddGraphqlFilters(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddType<UserSortType>()
            .AddType<UserFilterType>();
    }
}