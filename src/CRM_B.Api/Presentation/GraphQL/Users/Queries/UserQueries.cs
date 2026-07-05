using CRM_B.Api.Extensions.Auth;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Aggregates.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Api.Presentation.GraphQL.Users.Queries;

[QueryType, Authorize]
public sealed class UserQueries
{
    [UseSingleOrDefault, UseProjection, GraphQLName("profile")]
    public IQueryable<User> Profile(IDataContext db, IHttpContextAccessor http) =>
        db.Users.AsNoTracking().Where(u => u.Id == http.GetUserId());
}