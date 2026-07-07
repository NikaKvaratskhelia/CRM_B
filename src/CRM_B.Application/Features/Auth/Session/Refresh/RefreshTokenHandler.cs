using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Application.Contracts.Auth;
using CRM_B.Domain.Aggregates.Auth.Entities;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Auth.Session.Refresh;

public sealed class RefreshTokenHandler(IDataContext db, IUnitOfWork uow, IJwtService jwt, TimeProvider time)
    : ICommandHandler<RefreshTokenCommand, LoginOutcome>
{
    public async Task<Result<LoginOutcome>> Handle(RefreshTokenCommand req, CancellationToken ct)
    {
        var stored = await db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == req.RefreshToken, ct);

        var now = time.GetUtcNow().UtcDateTime;

        if (stored is { IsRevoked: true })
        {
            await RevokeAllForUserAsync(stored.UserId, now, ct);
            await uow.SaveChangesOnFailureAsync(ct);
            return Result<LoginOutcome>.Failure(AuthErrors.InvalidRefreshToken);
        }

        return await stored.EnsureFound(AuthErrors.InvalidRefreshToken)
            .Ensure(s => !s.IsExpiredAt(now), AuthErrors.InvalidRefreshToken)
            .BindAsync(s => Task.FromResult(Rotate(s, now)));
    }

    private Result<LoginOutcome> Rotate(RefreshToken stored, DateTime now)
    {
        var newToken = RefreshToken.Create(stored.UserId, now.Add(jwt.RefreshTokenLifetime));

        stored.RevokeAndReplace(newToken.Token, now);
        db.RefreshTokens.Add(newToken);

        var accessToken = jwt.GenerateAccessToken(stored.User);
        return Result<LoginOutcome>.Success(new LoginOutcome.Authenticated(
            accessToken,
            newToken.Token,
            new DateTimeOffset(newToken.ExpiresAt, TimeSpan.Zero)));
    }

    private async Task RevokeAllForUserAsync(UserId userId, DateTime now, CancellationToken ct)
    {
        var active = await db.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in active)
            token.Revoke(now);
    }
}