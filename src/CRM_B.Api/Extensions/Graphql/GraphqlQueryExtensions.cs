using CRM_B.Api.Presentation.GraphQL.Users.Queries;
using HotChocolate.Execution.Configuration;

namespace CRM_B.Api.Extensions.Graphql;

public static class GraphqlQueryExtensions
{
    public static IRequestExecutorBuilder AddGraphqlQueries(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddQueryType()
            .AddTypeExtension<UserQueries>();
    }
}