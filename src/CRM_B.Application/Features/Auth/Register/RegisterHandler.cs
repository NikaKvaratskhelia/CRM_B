using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Extensions;

namespace CRM_B.Application.Features.Auth.Register;

public sealed class RegisterHandler(IDataContext db, IPasswordHasher hasher, TimeProvider time)
    : ICommandHandler<RegisterCommand>
{
    public Task<Result> Handle(RegisterCommand req, CancellationToken ct)
    {
        var now = time.GetUtcNow().UtcDateTime;

        return Task.FromResult(
            User.Create(req.FullName, req.Email, hasher.Hash(req.Password))
                .Tap(u => u.CreateVerification(VerificationType.Email, now))
                .Tap(u => db.Users.Add(u))
                .ToResult());
    }
}