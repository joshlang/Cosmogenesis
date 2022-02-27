namespace Cosmogenesis.Core;

public sealed class DbUnexpectedStateException : DbException
{
    internal DbUnexpectedStateException()
    {
    }
    internal DbUnexpectedStateException(string message) : base(message)
    {
    }
    internal DbUnexpectedStateException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
