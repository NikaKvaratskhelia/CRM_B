using CRM_B.Domain.Aggregates.Users.Entities;
using HotChocolate.Data.Filters;

namespace CRM_B.Api.Presentation.GraphQL.Users.Filters;

public sealed class UserFilterType : FilterInputType<User>
{
    protected override void Configure(IFilterInputTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.FullName);
        descriptor.Field(x => x.Email);
        descriptor.Field(x => x.Role);
    }
}