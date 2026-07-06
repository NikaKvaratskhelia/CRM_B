using CRM_B.Domain.Aggregates.Users.Entities;
using HotChocolate.Data.Sorting;

namespace CRM_B.Api.Presentation.GraphQL.Users.Filters;

public sealed class UserSortType : SortInputType<User>
{
    protected override void Configure(ISortInputTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.FullName);
        descriptor.Field(x => x.CreatedAt);
    }
}