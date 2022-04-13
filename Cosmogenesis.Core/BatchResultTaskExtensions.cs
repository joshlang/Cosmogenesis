namespace Cosmogenesis.Core;

public static class BatchResultTaskExtensions
{
    public static async Task<List<DbDoc>> ThrowOnConflict(this Task<BatchResult> dbChangeTask)
    {
        var result = await dbChangeTask.ConfigureAwait(false);

        if (result.Conflict.HasValue)
        {
            throw new DbConflictException(result.Conflict.Value);
        }

        return result.Documents!;
    }
}
