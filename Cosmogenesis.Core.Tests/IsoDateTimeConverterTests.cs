using System;
using System.Text.Json;
using Cosmogenesis.Core.Converters;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public sealed class IsoDateTimeConverterTests
    {
        static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new IsoDateTimeConverter() }
        };

        class TestObj
        {
            public DateTime A { get; set; }
        }

        [Fact]
        public void Read_String_ReturnsValue()
        {
            var now = DateTime.UtcNow;
            foreach (var format in IsoDateCheater.GetFormats())
            {
                var formatted = now.ToString(format);
                var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""" + formatted + @"""}", JsonSerializerOptions);
                Assert.Equal(IsoDateCheater.Parse(formatted), obj?.A);
            }
        }

        [Fact]
        public void Read_EmptyString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions));

        [Fact]
        public void Read_InvalidString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""a""}", JsonSerializerOptions));

        [Fact]
        public void Read_NonString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

        [Fact]
        public void Write_Roundtrips()
        {
            var now = DateTime.UtcNow;
            var test = new TestObj { A = now };
            var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
            Assert.Contains(now.ToString("O"), s);
            var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
            Assert.Equal(obj?.A, test.A);
        }

        [Fact]
        public void Write_UsesOFormatting()
        {
            var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var test = new TestObj { A = date };
            var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
            Assert.Contains(date.ToString("O"), s);
            var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
            Assert.Equal(obj?.A, test.A);
        }
    }
}
