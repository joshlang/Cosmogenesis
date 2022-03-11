using Microsoft.Azure.Cosmos;

namespace Cosmogenesis.Core;

public abstract class ChangeFeedProcessorBase
{
    public static readonly TimeSpan DefaultPollInterval = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan MinPollInterval = TimeSpan.FromMilliseconds(500);
    public static readonly TimeSpan MaxPollInterval = TimeSpan.FromMinutes(5);

    public const int DefaultMaxItemsPerBatch = 100;
    public const int MaxMaxItemsPerBatch = 10000;

    ChangeFeedProcessor? changeFeedProcessor;
    protected virtual ChangeFeedProcessor ChangeFeedProcessor => changeFeedProcessor ??= CreateChangeFeedProcessor();

    protected virtual Container DatabaseContainer { get; } = default!;
    protected virtual Container LeaseContainer { get; } = default!;
    protected virtual string ProcessorName { get; } = default!;
    protected virtual TimeSpan PollInterval { get; } = default!;
    protected virtual int MaxItemsPerBatch { get; } = default!;
    protected virtual DateTime StartTime { get; } = default!;

    protected virtual BatchProcessor BatchProcessor { get; } = default!;

    protected ChangeFeedProcessorBase() { }

    protected ChangeFeedProcessorBase(
        Container databaseContainer,
        Container leaseContainer,
        string processorName,
        int maxItemsPerBatch,
        TimeSpan? pollInterval,
        DateTime? startTime,
        BatchProcessor batchProcessor)
    {
        if (string.IsNullOrWhiteSpace(processorName))
        {
            throw new ArgumentNullException(nameof(processorName));
        }

        if (maxItemsPerBatch < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxItemsPerBatch));
        }
        if (maxItemsPerBatch > MaxMaxItemsPerBatch)
        {
            maxItemsPerBatch = MaxMaxItemsPerBatch;
        }
        MaxItemsPerBatch = maxItemsPerBatch;

        PollInterval = pollInterval ?? DefaultPollInterval;
        if (PollInterval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(pollInterval));
        }
        if (PollInterval < MinPollInterval)
        {
            PollInterval = MinPollInterval;
        }
        else if (PollInterval > MaxPollInterval)
        {
            PollInterval = MaxPollInterval;
        }
        StartTime = startTime ?? IsoDateCheater.MinValue;
        BatchProcessor = batchProcessor ?? throw new ArgumentNullException(nameof(batchProcessor));
        ProcessorName = processorName;
        DatabaseContainer = databaseContainer ?? throw new ArgumentNullException(nameof(databaseContainer));
        LeaseContainer = leaseContainer ?? throw new ArgumentNullException(nameof(leaseContainer));
    }

    protected virtual ChangeFeedProcessor CreateChangeFeedProcessor() => DatabaseContainer
        .GetChangeFeedProcessorBuilder<DbDoc>(
            processorName: ProcessorName,
            onChangesDelegate: BatchProcessor.HandleAsync)
        .WithInstanceName($"{DateTime.UtcNow:O}-{Guid.NewGuid()}")
        .WithLeaseContainer(LeaseContainer)
        .WithPollInterval(PollInterval)
        .WithMaxItems(MaxItemsPerBatch)
        .WithStartTime(StartTime)
        .Build();

    /// <summary>
    /// Start processing the new and changed documents in SignedBlocksDb.
    /// Processing begins from where it left off if you use the same processor name.
    /// </summary>
    public virtual Task StartAsync() => ChangeFeedProcessor.StartAsync();

    /// <summary>
    /// Stop processing the new and changed documents in SignedBlocksDb.
    /// Processing will resume from where it left off if you use the same processor name.
    /// </summary>
    public virtual Task StopAsync() => ChangeFeedProcessor.StopAsync();
}
