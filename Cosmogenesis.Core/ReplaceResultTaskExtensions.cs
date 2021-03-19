using System.Threading.Tasks;

namespace Cosmogenesis.Core
{
    public static class ReplaceResultTaskExtensions
    {
        /// <summary>
        /// Returns the document, or throws DbConflictException if a conflict occurred.
        /// </summary>
        public static async Task<T> ThrowOnConflict<T>(this Task<ReplaceResult<T>> replaceResultTask) where T : DbDoc
        {
            var result = await replaceResultTask.ConfigureAwait(false);
            
            if (result.Conflict.HasValue)
            {
                throw new DbConflictException(result.Conflict.Value);
            }

            return result.Document!;
        }
    }
}
