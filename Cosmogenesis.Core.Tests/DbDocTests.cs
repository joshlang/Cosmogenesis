namespace Cosmogenesis.Core.Tests;

public class DbDocTests
{
    readonly DateTime Now = DateTime.UtcNow;

    [Fact]
    [Trait("Type", "Unit")]
    public void SetTypeTwice_NoChange_DoesNotThrow()
    {
        var t = new TestDoc
        {
            Type = "Asdf",
            _etag = "asdf"
        };
        t.Type = "Asdf";
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void SetTypeTwice_Change_Throws()
    {
        var t = new TestDoc
        {
            Type = "Asdf",
            _etag = "asdf"
        };
        Assert.ThrowsAny<Exception>(() => t.Type = "Explode");
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void SetCreationDateTwice_NoChange_DoesNotThrow()
    {
        var t = new TestDoc
        {
            CreationDate = Now,
            _etag = "asdf"
        };
        t.CreationDate = Now;
    }

    [Fact]
    [Trait("CreationDate", "Unit")]
    public void SetCreationDateTwice_Change_Throws()
    {
        var t = new TestDoc
        {
            CreationDate = Now,
            _etag = "asdf"
        };
        Assert.ThrowsAny<Exception>(() => t.CreationDate = Now.AddSeconds(1));
    }
}
