using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Auth.PasswordReset.Apply;

public sealed class ResetPasswordHandler(IDataContext db, IPasswordHasher hasher, TimeProvider time)
    : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Result> Handle(ResetPasswordCommand req, CancellationToken ct)
    {
        var verification = await db.Verifications
            .Include(u => u.User)
            .FirstOrDefaultAsync(u => u.Token == req.Token && u.Type == VerificationType.Password, ct);

        var now = time.GetUtcNow().UtcDateTime;

        return verification.EnsureFound(AuthErrors.PasswordResetNotFound)
            .Ensure(v => !v.IsExpiredAt(now), AuthErrors.PasswordResetExpired)
            .Tap(v =>
            {
                db.Verifications.Remove(v);
                v.User.ResetPassword(hasher.Hash(req.Password));
            })
            .ToResult();
    }
}