using System;

namespace CRM_B.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string GenerateToken(Guid userId, Guid? businessId, string email, string role);
}