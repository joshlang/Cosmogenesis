using System;

namespace Cosmogenesis.Core
{
    public sealed class DbOverloadedException : DbException
    {
        internal DbOverloadedException()
        {
        }
        internal DbOverloadedException(string message) : base(message)
        {
        }
        internal DbOverloadedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
