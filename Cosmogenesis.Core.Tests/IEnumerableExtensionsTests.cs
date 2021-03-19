using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_0MaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].ReuseableSegment(0).ToList());

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_NegativeMaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].ReuseableSegment(-1).ToList());

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_NullItems_ReturnsEmpty() => Assert.Empty(IEnumerableExtensions.ReuseableSegment<int>(null, 1));

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_EmptyItems_ReturnsEmpty() => Assert.Empty(IEnumerableExtensions.ReuseableSegment(new int[0], 1));

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_4Items1PerSegment_Returns4Segments() => Assert.Equal(4, new int[4].ReuseableSegment(1).Count());

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_4Items2PerSegment_Returns2Segments() => Assert.Equal(2, new int[4].ReuseableSegment(2).Count());
        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_4Items3PerSegment_Returns2Segments() => Assert.Equal(2, new int[4].ReuseableSegment(3).Count());

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_4Items4PerSegment_Returns1Segments() => Assert.Single(new int[4].ReuseableSegment(4));

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_4Items5PerSegment_Returns1Segments() => Assert.Single(new int[4].ReuseableSegment(5));

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_Items_FormSameCollection()
        {
            var rnd = new Random();
            var items = Enumerable.Range(0, 5 + rnd.Next() % 100).Select(x => rnd.Next()).ToArray();
            Assert.Equal(items, items.ReuseableSegment(1 + rnd.Next() % 10).SelectMany(x => x));
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ReuseableSegment_Items_SegmentsNotReused()
        {
            var segments = new int[2].ReuseableSegment(1).ToList();
            Assert.NotSame(segments[0], segments[1]);
        }



        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_0MaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].Segment(0).ToList());

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_NegativeMaxItems_ThrowsArgumentOutOfRangeExceptionWhenEnumerated() => Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].Segment(-1).ToList());

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_NullItems_ReturnsEmpty() => Assert.Empty(IEnumerableExtensions.Segment<int>(null, 1));

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_EmptyItems_ReturnsEmpty() => Assert.Empty(IEnumerableExtensions.Segment(new int[0], 1));

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_4Items1PerSegment_Returns4Segments() => Assert.Equal(4, new int[4].Segment(1).Select(x => x.Count()).Count());

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_4Items2PerSegment_Returns2Segments() => Assert.Equal(2, new int[4].Segment(2).Select(x => x.Count()).Count());
        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_4Items3PerSegment_Returns2Segments() => Assert.Equal(2, new int[4].Segment(3).Select(x => x.Count()).Count());

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_4Items4PerSegment_Returns1Segments() => Assert.Single(new int[4].Segment(4).Select(x => x.Count()));

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_4Items5PerSegment_Returns1Segments() => Assert.Single(new int[4].Segment(5).Select(x => x.Count()));

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_Items_FormSameCollection()
        {
            var rnd = new Random();
            var items = Enumerable.Range(0, 5 + rnd.Next() % 100).Select(x => rnd.Next()).ToArray();
            Assert.Equal(items, items.Segment(1 + rnd.Next() % 10).Select(x => x.ToList()).SelectMany(x => x));
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void Segment_1Items1PerSegmentNotEnumerated_ReturnsInfiniteSegments() => Assert.Equal(100, new int[1].Segment(1).Take(100).Count());

        [Fact]
        [Trait("Type", "Unit")]
        public void EmptyIfNull_Null_ReturnsEmpty() => Assert.Empty(((IEnumerable<int>?)null).EmptyIfNull());

        [Fact]
        [Trait("Type", "Unit")]
        public void EmptyIfNull_NonNull_ReturnsOriginal()
        {
            var b = new byte[2];
            Assert.Same(b, b.EmptyIfNull());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ExcludeNull_NoNulls_ReturnsOriginal()
        {
            var objs = new[] { new object(), new object() };
            Assert.Equal(objs, objs.ExcludeNull());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ExcludeNull_OnlyNulls_ReturnsEmpty()
        {
            var objs = new object?[] { null, null };
            Assert.Empty(objs.ExcludeNull());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ExcludeNull_SomeNulls_ReturnsNonNulls()
        {
            var objs = new object?[] { new object(), null, new object(), null };
            Assert.Equal(objs.Where(x => x != null), objs.ExcludeNull());
        }
    }
}
