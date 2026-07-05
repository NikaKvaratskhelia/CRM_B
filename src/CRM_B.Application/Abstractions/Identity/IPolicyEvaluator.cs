namespace CRM_B.Application.Abstractions.Identity;

public interface IPolicyEvaluator
{
    Task<bool> EvaluateAsync(string policy, CancellationToken ct);
}