namespace Cosmogenesis.Core;

public sealed class DbChange<T> where T : DbDoc
{
    internal static readonly DbChange<T> AlreadyExists = new(DbConflictType.AlreadyExists);
    internal static readonly DbChange<T> ETagChanged = new(DbConflictType.ETagChanged);
    internal static readonly DbChange<T> Missing = new(DbConflictType.Missing);
    internal static readonly DbChange<T> Null = new(null);

    internal DbChange(DbConflictType conflict)
    {
        if (conflict != DbConflictType.AlreadyExists &&
            conflict != DbConflictType.ETagChanged &&
            conflict != DbConflictType.Missing)
        {
            throw new ArgumentOutOfRangeException(nameof(conflict));
        }

        Conflict = conflict;
    }
    internal DbChange(T? document)
    {
        Document = document;
    }
    public T? Document { get; }
    public DbConflictType? Conflict { get; }
}
