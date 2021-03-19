using System.Net;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class DbUnknownStatusCodeExceptionTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_StatusCode_SetsField() => Assert.Equal(HttpStatusCode.AlreadyReported, new DbUnknownStatusCodeException(HttpStatusCode.AlreadyReported).StatusCode);
    }
}
