namespace Cosmogenesis.Core.Tests;

public class CreateOrReplaceResultTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void Ctor_PropertiesSet()
    {
        var result = new CreateOrReplaceResult<TestDoc>(TestDoc.Instance, true);
        Assert.True(result.AlreadyExisted);
        Assert.Same(TestDoc.Instance, result.Document);
    }
}
