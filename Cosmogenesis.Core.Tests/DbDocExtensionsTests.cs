using System;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class DbDocExtensionsTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public void GetApproxLastChangeDate_GivesCorrectDate()
        {
            var t = new TestDoc
            {
                _ts = RandomHelper.GetRandomPositiveInt32()
            };
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(t._ts).UtcDateTime, t.GetApproxLastChangeDate());
        }
    }
}
