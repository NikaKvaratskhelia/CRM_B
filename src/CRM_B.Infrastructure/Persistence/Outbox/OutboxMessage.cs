namespace CRM_B.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Type { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string CorrelationId { get; private set; } = string.Empty;
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }

    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    public static OutboxMessage Create(string type, string payload, string correlationId, DateTime occurredOn) => new()
    {
        Type = type,
        Payload = payload,
        CorrelationId = correlationId,
        OccurredOn = occurredOn,
    };

    public void MarkProcessed(DateTime now)
    {
        ProcessedOn = now;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        Error = error;
        RetryCount++;
    }
}