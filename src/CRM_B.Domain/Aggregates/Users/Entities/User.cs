using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.ValueObjects;

namespace CRM_B.Domain.Aggregates.Users.Entities;

public sealed class User
{
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

    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public UserRole Role { get; private set; } = UserRole.User;

    public DateTime CreatedAt { get; private set; } = new DateTime();
    public DateTime UpdatedAt { get; set; } = new DateTime();
}