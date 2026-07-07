using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Auth.EmailVerification.Resend;

public sealed class ResendEmailVerificationHandler(IDataContext db, TimeProvider time)
    : ICommandHandler<ResendEmailVerificationCommand>
{
    public async Task<Result> Handle(ResendEmailVerificationCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Verifications)
            .FirstOrDefaultAsync(u => u.Email.Value == req.Email, ct);

        var now = time.GetUtcNow().UtcDateTime;

        return user.EnsureFound(ErrorResults.NotFound("Email"))
            .Ensure(u => !u.IsVerified, AuthErrors.AlreadyVerified)
            .Tap(u =>
            {
                var existing = u.Verifications.Where(v => v.Type == VerificationType.Email).ToList();
                if (existing.Count > 0) db.Verifications.RemoveRange(existing);
                u.CreateVerification(VerificationType.Email, now);
            })
            .ToResult();
    }
}