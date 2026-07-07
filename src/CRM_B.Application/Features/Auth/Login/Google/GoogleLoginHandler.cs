using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Application.Contracts.Auth;
using CRM_B.Domain.Aggregates.Users.Entities;
using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Aggregates.Users.Errors;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Extensions;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = CRM_B.Domain.Aggregates.Auth.Entities.RefreshToken;

namespace CRM_B.Application.Features.Auth.Login.Google;

public sealed class GoogleLoginHandler(
    IDataContext db,
    IJwtService jwt,
    IGoogleAuthService google,
    TimeProvider time)
    : ICommandHandler<GoogleLoginCommand, LoginOutcome>
{
    public Task<Result<LoginOutcome>> Handle(GoogleLoginCommand req, CancellationToken ct) =>
        google.ValidateAsync(req.IdToken, ct)
            .BindAsync(googleUser => UpsertUserAsync(googleUser, ct))
            .BindAsync(user => Task.FromResult(IssueTokens(user)));

    private async Task<Result<User>> UpsertUserAsync(GoogleUser googleUser, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Email.Value == googleUser.Email, ct);

        if (user is not null)
            return user.AuthProvider == AuthProvider.Google
                ? Result<User>.Success(user)
                : Result<User>.Failure(UserErrors.AccountExistsWithDifferentProvider);

        return User
            .CreateExternal(googleUser.Name, googleUser.Email, AuthProvider.Google, googleUser.Subject)
            .Tap(u => db.Users.Add(u));
    }

    private Result<LoginOutcome> IssueTokens(User user)
    {
        var expiresAt = time.GetUtcNow().UtcDateTime.Add(jwt.RefreshTokenLifetime);
        var refreshToken = RefreshTokenEntity.Create(user.Id, expiresAt);
        db.RefreshTokens.Add(refreshToken);

        var accessToken = jwt.GenerateAccessToken(user);
        return Result<LoginOutcome>.Success(new LoginOutcome.Authenticated(
            accessToken,
            refreshToken.Token,
            new DateTimeOffset(refreshToken.ExpiresAt, TimeSpan.Zero)));
    }
}