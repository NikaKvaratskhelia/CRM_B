using System;
using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Primitives;
using CRM_B.Domain.ValueObjects;

namespace CRM_B.Domain.Aggregates.Users.Entities;

public sealed class User : BaseEntity
{
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public Role Role { get; private set; } = Role.User;

    private User()
    {
    }

    public User(Email email, string passwordHash, string firstName, string lastName)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FirstName = firstName;
        LastName = lastName;
    }
}