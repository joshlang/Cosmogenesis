namespace Cosmogenesis.Core;

public sealed class BatchResult
{
    internal static readonly BatchResult AlreadyExists = new(DbConflictType.AlreadyExists);
    internal static readonly BatchResult ETagChanged = new(DbConflictType.ETagChanged);
    internal static readonly BatchResult Missing = new(DbConflictType.Missing);

    internal BatchResult(DbConflictType conflict)
    {
        if (conflict != DbConflictType.AlreadyExists &&
            conflict != DbConflictType.ETagChanged &&
            conflict != DbConflictType.Missing)
        {
            throw new ArgumentOutOfRangeException(nameof(conflict));
        }

        Conflict = conflict;
    }
    internal BatchResult(int initialCapacity)
    {
        Documents = new(initialCapacity);
    }
    public List<DbDoc?>? Documents { get; }
    public DbConflictType? Conflict { get; }
}
