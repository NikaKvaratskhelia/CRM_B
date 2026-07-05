using CRM_B.Domain.Aggregates.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Abstractions.Persistence;

public interface IDataContext
{
    DbSet<User> Users { get; }
}