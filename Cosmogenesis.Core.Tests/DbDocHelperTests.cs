namespace Cosmogenesis.Core.Tests;

public class DbDocHelperTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void GetValidId_Null_Throws() => Assert.Throws<ArgumentNullException>(() => DbDocHelper.GetValidId(null!));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetValidId_TooLong_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => DbDocHelper.GetValidId(new string('a', DbDocHelper.MaxIdBytes + 1)));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetValidId_MaxLength_Returns() => Assert.Equal(new string('a', DbDocHelper.MaxIdBytes), DbDocHelper.GetValidId(new string('a', DbDocHelper.MaxIdBytes)));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetValidId_Empty_ReturnsEmpty() => Assert.Equal("", DbDocHelper.GetValidId(""));

    [Fact]
    [Trait("Type", "Unit")]
    public void GetValidId_InvalidChars_GetReplaced() => Assert.Equal("a____b", DbDocHelper.GetValidId("a/\\?#b"));
}
