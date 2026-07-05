using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Auth.Register;

internal sealed class RegisterHandler(IDataContext dbContext, IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterCommand, UserId>
{
    public async Task<UserId> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(u => u.Email.Value == request.Email, cancellationToken))
        {
            throw new Exception("იუზერი ამ ელ-ფოსტით უკვე არსებობს");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(
            new Email(request.Email),
            passwordHash,
            request.FirstName,
            request.LastName
        );

        dbContext.Users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}