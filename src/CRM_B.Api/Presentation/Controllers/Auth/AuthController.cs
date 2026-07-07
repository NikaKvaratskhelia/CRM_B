using CRM_B.Api.Contracts.Auth;
using CRM_B.Api.Hosting.Security;
using CRM_B.Api.RateLimiting;
using CRM_B.Application.Contracts.Auth;
using CRM_B.Application.Features.Auth.Session.Logout;
using CRM_B.Application.Features.Auth.Session.Refresh;
using CRM_B.Domain.Kernel.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CRM_B.Api.Presentation.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    private const string RefreshCookieName = "refresh_token";
    private const string RefreshCookiePath = "/api/v1/auth";

    [HttpPost("register")]
    [EnableRateLimiting(RateLimitPolicies.AuthRegister)]
    public async Task<IActionResult> Register(RegisterRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(), ct);
        return Ok(result);
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.AuthLogin)]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(), ct);
        return RespondWithSession(result);
    }

    [HttpPost("google-login")]
    [EnableRateLimiting(RateLimitPolicies.AuthLogin)]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GoogleLogin(GoogleLoginRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(), ct);
        return RespondWithSession(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting(RateLimitPolicies.AuthForgotPassword)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(), ct);
        return Ok(result);
    }

    [HttpPut("reset-password")]
    [EnableRateLimiting(RateLimitPolicies.AuthLogin)]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(), ct);
        return Ok(result);
    }

    [HttpPut("verify-email")]
    [EnableRateLimiting(RateLimitPolicies.AuthVerifyEmail)]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(), ct);
        return RespondWithSession(result);
    }

    [HttpPost("resend-verification")]
    [EnableRateLimiting(RateLimitPolicies.AuthResendVerification)]
    public async Task<IActionResult> ResendVerification(ResendVerificationRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req.ToCommand(), ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting(RateLimitPolicies.AuthRefresh)]
    [RequireXRequestedWith]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var token = Request.Cookies[RefreshCookieName] ?? string.Empty;
        var result = await mediator.Send(new RefreshTokenCommand(token), ct);
        return RespondWithSession(result);
    }

    [HttpPost("logout")]
    [EnableRateLimiting(RateLimitPolicies.AuthRefresh)]
    [RequireXRequestedWith]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var token = Request.Cookies[RefreshCookieName] ?? string.Empty;
        var result = await mediator.Send(new LogoutCommand(token), ct);
        ClearRefreshCookie();
        return Ok(result);
    }

    private IActionResult RespondWithSession(Result result)
    {
        if (!result.IsSuccess) return Ok(result);

        return result.GetValue() switch
        {
            LoginOutcome.Authenticated session => Authenticated(session),
            LoginOutcome.VerificationRequired => Ok(new AuthResponse(string.Empty, Verified: false)),
            _ => throw new InvalidOperationException(
                $"Unhandled LoginOutcome variant: {result.GetValue()?.GetType().Name}")
        };
    }

    private IActionResult Authenticated(LoginOutcome.Authenticated session)
    {
        SetRefreshCookie(session.RefreshToken, session.RefreshTokenExpiresAt);
        return Ok(new AuthResponse(session.AccessToken, Verified: true));
    }

    private void SetRefreshCookie(string token, DateTimeOffset expiresAt) =>
        Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = RefreshCookiePath,
            Expires = expiresAt,
        });

    private void ClearRefreshCookie() =>
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = RefreshCookiePath,
        });
}