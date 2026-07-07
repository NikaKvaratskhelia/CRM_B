using CRM_B.Application.Contracts.Auth;
using CRM_B.Domain.Kernel.Results;

namespace CRM_B.Application.Abstractions.Security;

public interface IGoogleAuthService
{
    Task<Result<GoogleUser>> ValidateAsync(string idToken, CancellationToken ct);
}