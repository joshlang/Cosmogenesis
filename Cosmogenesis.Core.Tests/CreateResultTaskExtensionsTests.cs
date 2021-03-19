using System.Threading.Tasks;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class CreateResultTaskExtensionsTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public Task ThrowOnConflict_Conflict_Throws() => Assert.ThrowsAsync<DbConflictException>(() => Task.FromResult(CreateResult<TestDoc>.AlreadyExists).ThrowOnConflict());

        [Fact]
        [Trait("Type", "Unit")]
        public async Task ThrowOnConflict_NoConflict_ReturnsResult() => Assert.Same(TestDoc.Instance, await Task.FromResult(new CreateResult<TestDoc>(TestDoc.Instance)).ThrowOnConflict());
    }
}
