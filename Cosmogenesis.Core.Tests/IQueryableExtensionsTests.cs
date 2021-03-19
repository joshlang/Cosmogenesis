using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class IQueryableExtensionsTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public void ForceCast_SameList()
        {
            var vals = new List<DbDoc> { TestDoc.Instance };
            var vals2 = vals.AsQueryable().ForceCast<TestDoc>().ToList();
            Assert.Equal(vals, vals2);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void AsKnownDocument_SameList()
        {
            var vals = new List<TestDoc> { TestDoc.Instance };
            var vals2 = vals.AsQueryable().AsKnownDocument().ToList();
            Assert.Equal(vals, vals2);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void AsDynamic_SameList()
        {
            var vals = new List<object> { new Dictionary<string, object>() };
            var vals2 = vals.AsQueryable().AsDynamic().ToList();
            Assert.Equal(vals, vals2);
        }
    }
}
