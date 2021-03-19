using System.Threading.Tasks;

namespace Cosmogenesis.Core
{
    public static class CreateResultTaskExtensions
    {
        /// <summary>
        /// Returns the document, or throws DbConflictException if a conflict occurred.
        /// </summary>
        public static async Task<T> ThrowOnConflict<T>(this Task<CreateResult<T>> createResultTask) where T : DbDoc
        {
            var result = await createResultTask.ConfigureAwait(false);
            
            if (result.Conflict.HasValue)
            {
                throw new DbConflictException(result.Conflict.Value);
            }

            return result.Document!;
        }
    }
}
