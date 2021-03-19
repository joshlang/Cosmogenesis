using System;

namespace Cosmogenesis.Core
{
    public sealed class ReadOrCreateResult<T> where T : DbDoc
    {
        /// <summary>
        /// The document that was read, or created if it did not already exist.
        /// </summary>
        public T Document { get; }

        /// <summary>
        /// True if the document was read (already existed).
        /// False if the document was created (did not exist).
        /// </summary>
        public bool AlreadyExisted { get; }

        internal ReadOrCreateResult(T document, bool alreadyExisted)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            AlreadyExisted = alreadyExisted;
        }
    }
}
