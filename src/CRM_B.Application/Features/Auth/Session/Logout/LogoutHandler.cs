using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Domain.Kernel.Results;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Auth.Session.Logout;

public sealed class LogoutHandler(IDataContext db, TimeProvider time) : ICommandHandler<LogoutCommand>
{
    public async Task<Result> Handle(LogoutCommand req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.RefreshToken)) return Result.Success();

        var stored = await db.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == req.RefreshToken, ct);

        if (stored is null || stored.IsRevoked) return Result.Success();

        stored.Revoke(time.GetUtcNow().UtcDateTime);
        return Result.Success();
    }
}