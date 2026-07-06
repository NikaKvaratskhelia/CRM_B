using CRM_B.Infrastructure.Jobs.Outbox.Cleanup;
using CRM_B.Infrastructure.Jobs.Outbox.Options;
using Hangfire;

namespace CRM_B.Infrastructure.Jobs.Outbox.Processing;

public static class OutboxJobScheduler
{
    public const string SafetyNetJobId = "outbox-safety-net";
    public const string CleanupJobId = "outbox-cleanup";

    public static void Schedule(IRecurringJobManager jobs, OutboxOptions options)
    {
        jobs.AddOrUpdate<OutboxProcessor>(SafetyNetJobId,
            p => p.ProcessPendingAsync(CancellationToken.None), options.SafetyNetCron);

        jobs.AddOrUpdate<OutboxCleanup>(CleanupJobId,
            c => c.RunAsync(CancellationToken.None), options.CleanupCron);
    }
}