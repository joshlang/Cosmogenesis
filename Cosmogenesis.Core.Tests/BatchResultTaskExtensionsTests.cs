using System.Threading.Tasks;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class BatchResultTaskExtensionsTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public Task ThrowOnConflict_Conflict_Throws() => Assert.ThrowsAsync<DbConflictException>(() => Task.FromResult(BatchResult.AlreadyExists).ThrowOnConflict());

        [Fact]
        [Trait("Type", "Unit")]
        public async Task ThrowOnConflict_NoConflict_DoesNotThrow() => await Task.FromResult(new BatchResult(0)).ThrowOnConflict();
    }
}
