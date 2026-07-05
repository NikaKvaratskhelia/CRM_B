using CRM_B.Domain.Aggregates.Users.Entities;

namespace CRM_B.Api.Presentation.GraphQL.Users.Types;

public sealed class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id).Type<NonNullType<StringType>>()
            .Resolve(c => c.Parent<User>().Id.Value);

        descriptor
            .Field(x => x.Email).Type<NonNullType<StringType>>()
            .Resolve(c => c.Parent<User>().Email.Value);

        descriptor.Field(x => x.Role);
        descriptor.Field(x => x.CreatedAt);
    }
}