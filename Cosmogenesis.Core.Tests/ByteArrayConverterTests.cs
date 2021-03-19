using System;
using System.Text.Json;
using Cosmogenesis.Core.Converters;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class ByteArrayConverterTests
    {
        static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new ByteArrayConverter() }
        };

        class TestObj
        {
            public byte[] A { get; set; } = default!;
        }

        [Fact]
        public void Read_String_ReturnsValue()
        {
            var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""0af8""}", JsonSerializerOptions);
            Assert.Equal(new byte[] { 0xa, 0xf8 }, obj?.A);
        }

        [Fact]
        public void Read_EmptyString_ReturnsEmptyArray()
        {
            var obj = JsonSerializer.Deserialize<TestObj>(@"{""A"":""""}", JsonSerializerOptions);
            Assert.Equal(new byte[] { }, obj?.A);
        }

        [Fact]
        public void Read_InvalidString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":""az""}", JsonSerializerOptions));

        [Fact]
        public void Read_NonString_Throws() => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<TestObj>(@"{""A"":1}", JsonSerializerOptions));

        [Fact]
        public void Write_Roundtrips()
        {
            var test = new TestObj { A = new byte[] { 0xe3, 0x70 } };
            var s = JsonSerializer.Serialize(test, JsonSerializerOptions);
            Assert.Contains(@"""e370""", s);
            var obj = JsonSerializer.Deserialize<TestObj>(s, JsonSerializerOptions);
            Assert.Equal(obj?.A, test.A);
        }
    }
}
