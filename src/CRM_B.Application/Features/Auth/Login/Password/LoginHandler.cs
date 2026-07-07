using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Application.Contracts.Auth;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Kernel.Results;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = CRM_B.Domain.Aggregates.Auth.Entities.RefreshToken;

namespace CRM_B.Application.Features.Auth.Login.Password;

public sealed class LoginHandler(
    IDataContext db,
    IUnitOfWork uow,
    IJwtService jwt,
    TimeProvider time,
    IPasswordHasher hasher
) : ICommandHandler<LoginCommand, LoginOutcome>
{
    public async Task<Result<LoginOutcome>> Handle(LoginCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Verifications)
            .FirstOrDefaultAsync(x => x.Email.Value == req.Email, ct);

        if (user is null)
            return Result<LoginOutcome>.Failure(AuthErrors.InvalidCredentials);

        var now = time.GetUtcNow().UtcDateTime;

        if (user.Lockout.IsLockedAt(now))
            return Result<LoginOutcome>.Failure(AuthErrors.AccountLocked);

        if (user.PasswordHash is null || !hasher.Verify(req.Password, user.PasswordHash))
        {
            user.RecordFailedLogin(now);

            await uow.SaveChangesOnFailureAsync(ct);

            return Result<LoginOutcome>.Failure(AuthErrors.InvalidCredentials);
        }

        return user.IsVerified ? IssueTokens(user, now) : RequestVerification(user, now);
    }

    private Result<LoginOutcome> IssueTokens(User user, DateTime now)
    {
        var refreshToken = RefreshTokenEntity.Create(user.Id, now.Add(jwt.RefreshTokenLifetime));
        db.RefreshTokens.Add(refreshToken);
        user.RecordSuccessfulLogin(now);

        var accessToken = jwt.GenerateAccessToken(user);

        return Result<LoginOutcome>.Success(new LoginOutcome.Authenticated(accessToken, refreshToken.Token,
            new DateTimeOffset(refreshToken.ExpiresAt, TimeSpan.Zero)));
    }

    private Result<LoginOutcome> RequestVerification(User user, DateTime now)
    {
        var existing = user.Verifications.Where(v => v.Type == VerificationType.Email).ToList();
        if (existing.Count > 0) db.Verifications.RemoveRange(existing);

        user.CreateVerification(VerificationType.Email, now);

        return Result<LoginOutcome>.Success(LoginOutcome.VerificationRequired.Instance);
    }
}