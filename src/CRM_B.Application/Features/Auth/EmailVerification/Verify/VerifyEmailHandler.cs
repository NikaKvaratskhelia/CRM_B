using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Application.Contracts.Auth;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = CRM_B.Domain.Aggregates.Auth.Entities.RefreshToken;

namespace CRM_B.Application.Features.Auth.EmailVerification.Verify;

public sealed class VerifyEmailHandler(IDataContext db, IJwtService jwt, TimeProvider time)
    : ICommandHandler<VerifyEmailCommand, LoginOutcome>
{
    public async Task<Result<LoginOutcome>> Handle(VerifyEmailCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Verifications)
            .FirstOrDefaultAsync(u => u.Email.Value == req.Email, ct);

        var now = time.GetUtcNow().UtcDateTime;

        return user.EnsureFound(ErrorResults.NotFound("User"))
            .Ensure(u => !u.IsVerified, AuthErrors.AlreadyVerified)
            .Bind(u => VerifyOtp(u, req.Otp, now))
            .Bind(u => Complete(u, now));
    }

    private static Result<User> VerifyOtp(User user, string otp, DateTime now)
    {
        var verification = user.Verifications.FirstOrDefault(v => v.Type == VerificationType.Email);

        if (verification is null) return Result<User>.Failure(AuthErrors.VerificationNotFound);
        if (verification.Otp != otp) return Result<User>.Failure(AuthErrors.VerificationCodeIncorrect);
        if (verification.IsExpiredAt(now)) return Result<User>.Failure(AuthErrors.VerificationCodeExpired);

        return Result<User>.Success(user);
    }

    private Result<LoginOutcome> Complete(User user, DateTime now)
    {
        var verification = user.Verifications.First(v => v.Type == VerificationType.Email);

        db.Verifications.Remove(verification);

        user.Verify();

        var refreshToken = RefreshTokenEntity.Create(user.Id, now.Add(jwt.RefreshTokenLifetime));
        db.RefreshTokens.Add(refreshToken);

        var accessToken = jwt.GenerateAccessToken(user);
        return Result<LoginOutcome>.Success(new LoginOutcome.Authenticated(
            accessToken,
            refreshToken.Token,
            new DateTimeOffset(refreshToken.ExpiresAt, TimeSpan.Zero)));
    }
}