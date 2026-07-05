namespace CRM_B.Application.Abstractions.Messaging.Authorization;

public interface IRequireAuthorization
{
    string? Policy => null;
}