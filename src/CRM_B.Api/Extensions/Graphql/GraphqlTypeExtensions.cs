using CRM_B.Api.Presentation.GraphQL.Users.Types;
using HotChocolate.Execution.Configuration;

namespace CRM_B.Api.Extensions.Graphql;

public static class GraphqlTypeExtensions
{
    public static IRequestExecutorBuilder AddGraphqlTypes(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddType<UserType>();
    }
}