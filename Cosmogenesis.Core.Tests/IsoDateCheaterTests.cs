using System;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class IsoDateCheaterTests
    {
        static readonly string[] Formats = new[]
        {
            "yyyy-MM-dd'T'HH:mm:ss'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.f'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.ff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.ffff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fffff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'"
        };

        [Fact]
        [Trait("Type", "Unit")]
        public void GetFormats_ReturnsExpectedValues() => Assert.Equal(Formats, IsoDateCheater.GetFormats());

        [Fact]
        [Trait("Type", "Unit")]
        public void GetFormat_ReturnsExpectedValues()
        {
            for (var x = 0; x < Formats.Length; ++x)
            {
                Assert.Equal(Formats[x], IsoDateCheater.GetFormat(x));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void GetFormat_Negative_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => IsoDateCheater.GetFormat(-1));

        [Fact]
        [Trait("Type", "Unit")]
        public void GetFormat_TooHigh_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => IsoDateCheater.GetFormat(-1));

        [Fact]
        [Trait("Type", "Unit")]
        public void TryParse_EachFormat_ResultsMatch()
        {
            for (var x = 0; x < Formats.Length; ++x)
            {
                var now = DateTime.UtcNow;
                var nowString = now.ToString(Formats[x]);
                Assert.True(IsoDateCheater.TryParse(nowString, out var date));
                Assert.Equal(nowString, date.ToString(Formats[x]));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void TryParse_Null_ReturnsFalse() => Assert.False(IsoDateCheater.TryParse(null, out _));

        [Fact]
        [Trait("Type", "Unit")]
        public void TryParse_InvalidText_ReturnsFalse() => Assert.False(IsoDateCheater.TryParse("asdfoiasfd", out _));

        [Fact]
        [Trait("Type", "Unit")]
        public void TryParse_ValidDateWrongFormat_ReturnsFalse() => Assert.False(IsoDateCheater.TryParse(DateTime.UtcNow.ToString("u"), out _));

        [Fact]
        [Trait("Type", "Unit")]
        public void TryParse_o_ReturnsTrue() => Assert.True(IsoDateCheater.TryParse(DateTime.UtcNow.ToString("O"), out _));

        [Fact]
        [Trait("Type", "Unit")]
        public void Parse_ToString_RoundtripWorks()
        {
            var date = DateTime.UtcNow;
            var newDate = IsoDateCheater.Parse(date.ToString("O"));
            Assert.Equal(newDate, date);
            Assert.Equal(newDate.ToString("O"), date.ToString("O"));
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void Parse_ReturnsUtc()
        {
            var newDate = IsoDateCheater.Parse(DateTime.UtcNow.ToString("O"));
            Assert.Equal(DateTimeKind.Utc, newDate.Kind);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void MinValue_0UTC()
        {
            Assert.Equal(0, IsoDateCheater.MinValue.Ticks);
            Assert.Equal(DateTimeKind.Utc, IsoDateCheater.MinValue.Kind);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void MaxValue_MaxUTC()
        {
            Assert.Equal(DateTime.MaxValue.Ticks, IsoDateCheater.MaxValue.Ticks);
            Assert.Equal(DateTimeKind.Utc, IsoDateCheater.MaxValue.Kind);
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void MinValue_Equivalent() => Assert.Equal(DateTime.MinValue, IsoDateCheater.MinValue);

        [Fact]
        [Trait("Type", "Unit")]
        public void MaxValue_Equivalent() => Assert.Equal(DateTime.MaxValue, IsoDateCheater.MaxValue);
    }
}
