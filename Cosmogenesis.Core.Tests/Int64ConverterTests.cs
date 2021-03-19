using System;
using System.Text.Json;
using Cosmogenesis.Core.Converters;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class Int64ConverterTests
    {
        static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new Int64Converter() }
        };

        class TestObj
        {
            public long A { get; set; }
        }

        [Fact]
        public void Read_String_ReturnsValue()
        {
            var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""123""}", JsonSerializerOptions);
            Assert.Equal(123, obj?.A);
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
            var test = new TestObj { A = -123 };
            var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
            Assert.Contains(@"""-123""", s);
            var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
            Assert.Equal(obj?.A, test.A);
        }
    }
}
