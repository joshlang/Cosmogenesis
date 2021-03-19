using System;
using System.Collections;
using System.Linq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class TypeExtensionsTests
    {
        interface IMeow
        {
        }
        interface IRawr<T>
        {
        }
        class TestObj : IMeow, IRawr<int>, IRawr<string>
        {
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void GetGenericInterfaces_TestObj_ReturnsBothInterfaces()
        {
            var interfaces = typeof(TestObj).GetGenericInterfaces(typeof(IRawr<>)).ToList();
            Assert.Contains(typeof(IRawr<int>), interfaces);
            Assert.Contains(typeof(IRawr<string>), interfaces);
            Assert.Equal(2, interfaces.Count);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void GetGenericInterfaces_NullType_Throws() => Assert.Throws<ArgumentNullException>(() => ((Type)null!).GetGenericInterfaces(typeof(IRawr<>)));

        [Fact]
        [Trait("Type", "Unit")]
        public void GetGenericInterfaces_NullInterface_Throws() => Assert.Throws<ArgumentNullException>(() => typeof(TestObj).GetGenericInterfaces(null!));

        [Fact]
        [Trait("Type", "Unit")]
        public void GetGenericInterfaces_NonGenericInterface_Throws() => Assert.Throws<InvalidOperationException>(() => typeof(TestObj).GetGenericInterfaces(typeof(IEnumerable)));

        [Fact]
        [Trait("Type", "Unit")]
        public void GetGenericInterfaces_NonInterface_Throws() => Assert.Throws<InvalidOperationException>(() => typeof(TestObj).GetGenericInterfaces(typeof(object)));
    }
}
