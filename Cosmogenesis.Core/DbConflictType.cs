namespace Cosmogenesis.Core
{
    public enum DbConflictType
    {
        /// <summary>
        /// A document with the same pk/id already exists
        /// </summary>
        AlreadyExists,
        /// <summary>
        /// The document still exists but the ETag has changed
        /// </summary>
        ETagChanged,
        /// <summary>
        /// The document no longer exists
        /// </summary>
        Missing
    }
}
