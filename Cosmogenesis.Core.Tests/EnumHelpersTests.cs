namespace Cosmogenesis.Core.Tests;

public class EnumHelpersTests
{
    enum TestEnum
    {
        Apples,
        Bananas = 4,
#pragma warning disable CA1069 // Enums values should not be duplicated
        Oranges = 4,
#pragma warning restore CA1069 // Enums values should not be duplicated
        OtherFruit = 10
    }

    [Fact]
    public void Values_TestEnum_ReturnsAllEnums() => Assert.Equal(
        new[] { TestEnum.Apples, TestEnum.Bananas, TestEnum.Oranges, TestEnum.OtherFruit }.OrderBy(x => x.ToString()),
        EnumHelper<TestEnum>.Values.OrderBy(x => x.ToString()));
}
