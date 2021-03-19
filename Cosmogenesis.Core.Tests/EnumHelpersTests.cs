using System.Linq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class EnumHelpersTests
    {
        enum TestEnum
        {
            Apples,
            Bananas = 4,
            Oranges = 4,
            OtherFruit = 10
        }

        [Fact]
        public void Values_TestEnum_ReturnsAllEnums() => Assert.Equal(
            new[] { TestEnum.Apples, TestEnum.Bananas, TestEnum.Oranges, TestEnum.OtherFruit }.OrderBy(x => x.ToString()),
            EnumHelper<TestEnum>.Values.OrderBy(x => x.ToString()));
    }
}
