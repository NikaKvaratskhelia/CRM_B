using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Kernel.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Auth.PasswordReset.Request;

public sealed class SendPasswordResetEmailHandler(IDataContext db, TimeProvider time)
    : ICommandHandler<SendPasswordResetEmailCommand>
{
    public async Task<Result> Handle(SendPasswordResetEmailCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Verifications)
            .FirstOrDefaultAsync(x => x.Email.Value == req.Email, ct);

        if (user is null) return Result.Success();

        var existing = user.Verifications.Where(v => v.Type == VerificationType.Password).ToList();
        if (existing.Count > 0) db.Verifications.RemoveRange(existing);

        user.CreateVerification(VerificationType.Password, time.GetUtcNow().UtcDateTime);

        return Result.Success();
    }
}