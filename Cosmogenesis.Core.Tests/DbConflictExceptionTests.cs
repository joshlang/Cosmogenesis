namespace Cosmogenesis.Core.Tests;

public class DbConflictExceptionTests
{
    [Fact]
    [Trait("Type", "Unit")]
    public void Ctor_Conflict_FieldSet() => Assert.Equal(DbConflictType.Missing, new DbConflictException(DbConflictType.Missing).DbConflictType);
}
