using CRM_B.Application.Abstractions.Security;
using CRM_B.Application.Contracts.Auth;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Kernel.Results;
using Google.Apis.Auth;

namespace CRM_B.Infrastructure.Security.Google;

public sealed class GoogleService(GoogleOptions options) : IGoogleAuthService
{
    public async Task<Result<GoogleUser>> ValidateAsync(string idToken, CancellationToken ct)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [options.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return Result<GoogleUser>.Success(new GoogleUser(payload.Email, payload.Name, payload.Subject));
        }
        catch (InvalidJwtException)
        {
            return Result<GoogleUser>.Failure(AuthErrors.InvalidCredentials);
        }
    }
}