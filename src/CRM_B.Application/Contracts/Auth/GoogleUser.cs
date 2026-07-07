namespace CRM_B.Application.Contracts.Auth;

public sealed record GoogleUser(string Email, string Name, string Subject);