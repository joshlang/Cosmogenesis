namespace Cosmogenesis.Core.Tests;

public class CreateResultTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void Ctor_Conflict_AllowedValues()
    {
        var allowed = new[]
        {
            DbConflictType.AlreadyExists
        };
        foreach (var conflict in EnumHelper<DbConflictType>.Values)
        {
            if (allowed.Contains(conflict))
            {
                Assert.Equal(conflict, new CreateResult<TestDoc>(conflict).Conflict);
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new CreateResult<TestDoc>(conflict));
            }
        }
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void AlreadyExists_CorrectConflict()
    {
        Assert.Null(CreateResult<TestDoc>.AlreadyExists.Document);
        Assert.Equal(DbConflictType.AlreadyExists, CreateResult<TestDoc>.AlreadyExists.Conflict);
    }

    [Fact]
    [Trait("Type", "Unit")]
    public void Ctor_Doc_HasDoc()
    {
        Assert.Null(new CreateResult<TestDoc>(TestDoc.Instance).Conflict);
        Assert.Same(TestDoc.Instance, new CreateResult<TestDoc>(TestDoc.Instance).Document);
    }
}
