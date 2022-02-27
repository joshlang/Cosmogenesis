namespace Cosmogenesis.Core;

public sealed class DbRequestTooLargeException : DbException
{
    internal DbRequestTooLargeException()
    {
    }
    internal DbRequestTooLargeException(string message) : base(message)
    {
    }
    internal DbRequestTooLargeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
