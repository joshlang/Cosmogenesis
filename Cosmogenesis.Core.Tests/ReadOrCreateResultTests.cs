namespace Cosmogenesis.Core.Tests;

public class ReadOrCreateResultTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void Ctor_PropertiesSet()
    {
        var result = new ReadOrCreateResult<TestDoc>(TestDoc.Instance, true);
        Assert.True(result.AlreadyExisted);
        Assert.Same(TestDoc.Instance, result.Document);
    }
}
