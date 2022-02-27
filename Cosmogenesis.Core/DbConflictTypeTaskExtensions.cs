namespace Cosmogenesis.Core;

public static class DbConflictTypeTaskExtensions
{
    public static async Task ThrowOnConflict(this Task<DbConflictType?> dbConflictTypeTask)
    {
        var result = await dbConflictTypeTask.ConfigureAwait(false);

        if (result.HasValue)
        {
            throw new DbConflictException(result.Value);
        }
    }
}
