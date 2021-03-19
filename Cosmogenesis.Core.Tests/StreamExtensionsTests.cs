using System;
using System.IO;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class StreamExtensionsTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public void ToSpan_GetsSpan()
        {
            var ms = new MemoryStream(RandomHelper.GetRandomBytes(123));
            var s = ms.ToSpan();
            Assert.Equal(s.Length, ms.Length);
            Assert.True(s.SequenceEqual(ms.ToArray()));
        }
    }
}
