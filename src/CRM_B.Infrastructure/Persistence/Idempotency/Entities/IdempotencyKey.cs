namespace CRM_B.Infrastructure.Persistence.Idempotency.Entities;

public sealed class IdempotencyKey
{
    private IdempotencyKey()
    {
    }

    public string Key { get; private set; } = string.Empty;
    public string RequestHash { get; private set; } = string.Empty;
    public string ResponsePayload { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public static IdempotencyKey Create(string key, string requestHash, string responsePayload,
        DateTime createdAt, DateTime expiresAt) => new()
    {
        Key = key,
        RequestHash = requestHash,
        ResponsePayload = responsePayload,
        CreatedAt = createdAt,
        ExpiresAt = expiresAt,
    };
}