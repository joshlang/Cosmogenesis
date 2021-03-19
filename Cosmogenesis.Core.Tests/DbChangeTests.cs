using System;
using System.Linq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class DbChangeTests
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
                    Assert.Equal(conflict, new DbChange<TestDoc>(conflict).Conflict);
                }
                else
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => new DbChange<TestDoc>(conflict));
                }
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void AlreadyExists_CorrectConflict()
        {
            Assert.Null(DbChange<TestDoc>.AlreadyExists.Document);
            Assert.Equal(DbConflictType.AlreadyExists, DbChange<TestDoc>.AlreadyExists.Conflict);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ETagChanged_CorrectConflict()
        {
            Assert.Null(DbChange<TestDoc>.ETagChanged.Document);
            Assert.Equal(DbConflictType.ETagChanged, DbChange<TestDoc>.ETagChanged.Conflict);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void Missing_CorrectConflict()
        {
            Assert.Null(DbChange<TestDoc>.Missing.Document);
            Assert.Equal(DbConflictType.Missing, DbChange<TestDoc>.Missing.Conflict);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void Null_CorrectConflict()
        {
            Assert.Null(DbChange<TestDoc>.Null.Document);
            Assert.Null(DbChange<TestDoc>.Null.Conflict);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void Ctor_Doc_HasDoc()
        {
            Assert.Null(new DbChange<TestDoc>(TestDoc.Instance).Conflict);
            Assert.Same(TestDoc.Instance, new DbChange<TestDoc>(TestDoc.Instance).Document);
        }
    }
}
