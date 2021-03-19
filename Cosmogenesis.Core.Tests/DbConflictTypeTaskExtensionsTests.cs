using System.Threading.Tasks;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class DbConflictTypeTaskExtensionsTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public Task ThrowOnConflict_Conflict_Throws() => Assert.ThrowsAsync<DbConflictException>(() => Task.FromResult((DbConflictType?)DbConflictType.Missing).ThrowOnConflict());

        [Fact]
        [Trait("Type", "Unit")]
        public async Task ThrowOnConflict_NoConflict_DoesNotThrow() => await Task.FromResult((DbConflictType?)null).ThrowOnConflict();
    }
}
