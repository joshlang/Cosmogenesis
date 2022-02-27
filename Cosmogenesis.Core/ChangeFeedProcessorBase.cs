using Microsoft.Azure.Cosmos;

namespace Cosmogenesis.Core;

public abstract class ChangeFeedProcessorBase
{
    public static readonly TimeSpan DefaultPollInterval = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan MinPollInterval = TimeSpan.FromMilliseconds(500);
    public static readonly TimeSpan MaxPollInterval = TimeSpan.FromMinutes(5);

    public const int DefaultMaxItemsPerBatch = 100;
    public const int MaxMaxItemsPerBatch = 10000;

    public const ChangeFeedProcessingMode DefaultProcessingMode = ChangeFeedProcessingMode.SequentialByPartition;

    public virtual ChangeFeedProcessingMode ProcessingMode { get; set; } = DefaultProcessingMode;

    ChangeFeedProcessor? changeFeedProcessor;
    protected virtual ChangeFeedProcessor ChangeFeedProcessor => changeFeedProcessor ??= CreateChangeFeedProcessor();

    protected virtual Container DatabaseContainer { get; } = default!;
    protected virtual Container LeaseContainer { get; } = default!;
    protected virtual string ProcessorName { get; } = default!;
    protected virtual TimeSpan PollInterval { get; } = default!;
    protected virtual int MaxItemsPerBatch { get; } = default!;
    protected virtual DateTime StartTime { get; } = default!;

    protected virtual ChangeFeedHandlersBase ChangeFeedHandlers { get; } = default!;

    protected ChangeFeedProcessorBase() { }

    protected ChangeFeedProcessorBase(
        Container databaseContainer,
        Container leaseContainer,
        string processorName,
        int maxItemsPerBatch,
        TimeSpan? pollInterval,
        DateTime? startTime,
        ChangeFeedHandlersBase changeFeedHandlers)
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
        ChangeFeedHandlers = changeFeedHandlers ?? throw new ArgumentNullException(nameof(changeFeedHandlers));
        ProcessorName = processorName;
        DatabaseContainer = databaseContainer ?? throw new ArgumentNullException(nameof(databaseContainer));
        LeaseContainer = leaseContainer ?? throw new ArgumentNullException(nameof(leaseContainer));
    }

    protected virtual ChangeFeedProcessor CreateChangeFeedProcessor() => DatabaseContainer
        .GetChangeFeedProcessorBuilder<DbDoc>(
            processorName: ProcessorName,
            onChangesDelegate: ChangesHandler)
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

    protected virtual async Task ChangesHandler(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken)
    {
        if (changes is null || changes.Count == 0)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (ChangeFeedHandlers.NewChangeFeedBatch?.Invoke(cancellationToken) is Task newChangeFeedBatch)
        {
            await newChangeFeedBatch.ConfigureAwait(false);
        }

        await (ProcessingMode switch
        {
            ChangeFeedProcessingMode.AllAtOnce => HandleAllAtOnce(changes: changes, cancellationToken: cancellationToken),
            ChangeFeedProcessingMode.SequentialByPartition => HandleSequentialByPartition(changes: changes, cancellationToken: cancellationToken),
            ChangeFeedProcessingMode.Sequential => HandleSequential(changes: changes, cancellationToken: cancellationToken),
            _ => throw new NotImplementedException()
        }).ConfigureAwait(false);

        if (ChangeFeedHandlers.FinishingBatch?.Invoke(cancellationToken) is Task finishingBatch)
        {
            await finishingBatch.ConfigureAwait(false);
        }
    }

    protected virtual Task HandleAllAtOnce(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken) => Task.WhenAll(changes.Select(x => GetHandlerTask(x, cancellationToken)).ExcludeNull());

    protected virtual Task HandleSequentialByPartition(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken)
    {
        var plan = changes
            .Select((doc, index) => new
            {
                doc,
                index
            })
            .GroupBy(x => x.doc.pk)
            .Select(x => x.OrderBy(x => x.index).ToList())
            .OrderBy(x => x[0].index)
            .ToList();

        static async Task ProcessInOrder(IEnumerable<DbDoc> docs, CancellationToken cancellationToken, Func<DbDoc, CancellationToken, Task?> getHandlerTask)
        {
            foreach (var doc in docs)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var t = getHandlerTask(doc, cancellationToken);
                if (t != null)
                {
                    await t.ConfigureAwait(false);
                }
            }
        }

        return Task.WhenAll(plan.Select(x => ProcessInOrder(x.Select(a => a.doc), cancellationToken, GetHandlerTask)));
    }

    protected virtual async Task HandleSequential(IReadOnlyCollection<DbDoc> changes, CancellationToken cancellationToken)
    {
        foreach (var doc in changes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var t = GetHandlerTask(doc, cancellationToken);
            if (t != null)
            {
                await t.ConfigureAwait(false);
            }
        }
    }

    protected abstract Task? GetHandlerTask(DbDoc change, CancellationToken cancellationToken);
}
