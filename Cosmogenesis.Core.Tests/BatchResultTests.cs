using System;
using System.Linq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class BatchResultTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_Conflict_AllowedValues()
        {
            var allowed = new[]
            {
                DbConflictType.AlreadyExists,
                DbConflictType.ETagChanged,
                DbConflictType.Missing
            };
            foreach (var conflict in EnumHelper<DbConflictType>.Values)
            {
                if (allowed.Contains(conflict))
                {
                    Assert.Equal(conflict, new BatchResult(conflict).Conflict);
                }
                else
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => new BatchResult(conflict));
                }
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void AlreadyExists_CorrectConflict()
        {
            Assert.Null(BatchResult.AlreadyExists.Documents);
            Assert.Equal(DbConflictType.AlreadyExists, BatchResult.AlreadyExists.Conflict);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ETagChanged_CorrectConflict()
        {
            Assert.Null(BatchResult.ETagChanged.Documents);
            Assert.Equal(DbConflictType.ETagChanged, BatchResult.ETagChanged.Conflict);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void Missing_CorrectConflict()
        {
            Assert.Null(BatchResult.Missing.Documents);
            Assert.Equal(DbConflictType.Missing, BatchResult.Missing.Conflict);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_Capacity_NoConflict()
        {
            Assert.Null(new BatchResult(0).Conflict);
            Assert.NotNull(new BatchResult(0).Documents);
        }
    }
}
