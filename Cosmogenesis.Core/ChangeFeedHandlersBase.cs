namespace Cosmogenesis.Core;

public abstract class ChangeFeedHandlersBase
{
    public virtual Func<CancellationToken, Task>? NewChangeFeedBatch { get; set; }
    public virtual Func<CancellationToken, Task>? FinishingBatch { get; set; }
}
