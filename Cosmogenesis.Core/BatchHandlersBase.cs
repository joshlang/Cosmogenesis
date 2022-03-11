namespace Cosmogenesis.Core;

public abstract class BatchHandlersBase
{
    public virtual Func<CancellationToken, Task>? NewChangeFeedBatch { get; set; }
    public virtual Func<CancellationToken, Task>? FinishingBatch { get; set; }

    public abstract Task? GetHandlerTask(DbDoc change, CancellationToken cancellationToken = default);
}
