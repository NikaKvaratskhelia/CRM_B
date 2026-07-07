namespace CRM_B.Api.Contracts.Auth;

public sealed record AuthResponse(string AccessToken, bool Verified);