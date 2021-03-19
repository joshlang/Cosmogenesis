using System.Net;

namespace Cosmogenesis.Core
{
    public sealed class DbUnknownStatusCodeException : DbException
    {
        internal DbUnknownStatusCodeException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}
