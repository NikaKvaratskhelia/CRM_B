using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Users.ChangePassword;

public sealed class ChangePasswordHandler(IDataContext db, IPasswordHasher hasher)
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == req.UserId, ct);

        return user.EnsureFound(ErrorResults.NotFound("User"))
            .Ensure(u => u.PasswordHash is not null && hasher.Verify(req.OldPassword, u.PasswordHash),
                AuthErrors.InvalidCredentials)
            .Tap(u => u.SetPasswordHash(hasher.Hash(req.NewPassword)))
            .ToResult();
    }
}