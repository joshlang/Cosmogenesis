using System;
using System.Buffers;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Cosmogenesis.Core.Tests
{
    public class StringExtensionsTests
    {
        static byte[] CrappyToBytes(string str)
        {
            var result = new byte[str.Length / 2];
            for (var x = 0; x < result.Length; ++x)
            {
                result[x] = byte.Parse(str.Substring(x * 2, 2), NumberStyles.HexNumber);
            }
            return result;
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHex_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((byte[])null!).ToLowerHex());

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHex_AllBytes_ReturnHexString()
        {
            for (var x = 0; x < 256; ++x)
            {
                Assert.Equal(new byte[] { (byte)x }.ToLowerHex(), x.ToString("x2"));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHex_AllBytesSpan_ReturnHexString()
        {
            for (var x = 0; x < 256; ++x)
            {
                Assert.Equal(new byte[] { (byte)x }.AsSpan().ToLowerHex(), x.ToString("x2"));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHex_RandomBytes_ReturnHexString()
        {
            var buf = RandomHelper.GetRandomBytes(32);
            Assert.Equal(buf.ToLowerHex(), string.Concat(buf.Select(x => x.ToString("x2"))));
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHex_RandomBytesSpan_ReturnHexString()
        {
            var buf = RandomHelper.GetRandomBytes(32);
            Assert.Equal(buf.AsSpan().ToLowerHex(), string.Concat(buf.Select(x => x.ToString("x2"))));
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void TryToBigInteger_Null_ReturnsNull() => Assert.Null(((string)null!).TryToBigInteger());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToInt64_Null_ReturnsNull() => Assert.Null(((string)null!).TryToInt64());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToInt32_Null_ReturnsNull() => Assert.Null(((string)null!).TryToInt32());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToDouble_Null_ReturnsNull() => Assert.Null(((string)null!).TryToDouble());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToDecimal_Null_ReturnsNull() => Assert.Null(((string)null!).TryToDecimal());


        [Fact]
        [Trait("Type", "Unit")]
        public void TryToBigInteger_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToBigInteger());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToInt64_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToInt64());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToInt32_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToInt32());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToDouble_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToDouble());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToDecimal_InvalidNumber_ReturnsNull() => Assert.Null("A".TryToDecimal());

        [Fact]
        [Trait("Type", "Unit")]
        public void TryToBigInteger_Returns_CorrectValue() => Assert.Equal("1234", "1234".TryToBigInteger().ToString());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToInt64_Returns_CorrectValue() => Assert.Equal("1234", "1234".TryToInt64().ToString());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToInt32_Returns_CorrectValue() => Assert.Equal("1234", "1234".TryToInt32().ToString());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToDouble_Returns_CorrectValue() => Assert.Equal("123.4", "123.4".TryToDouble().ToString());
        [Fact]
        [Trait("Type", "Unit")]
        public void TryToDecimal_Returns_CorrectValue() => Assert.Equal("123.4", "123.4".TryToDecimal().ToString());

        [Fact]
        [Trait("Type", "Unit")]
        public void ToHexBytes_Null_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((string)null!).ToHexBytes());

        [Fact]
        [Trait("Type", "Unit")]
        public void ToHexBytes_OddLengthString_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => "123".ToHexBytes());

        [Theory]
        [InlineData("z1")]
        [InlineData("1z")]
        [InlineData(" 0")]
        [Trait("Type", "Unit")]
        public void ToHexBytes_BadString_ThrowsArgumentOutOfRangeException(string str) => Assert.Throws<ArgumentOutOfRangeException>(() => str.ToHexBytes());

        [Fact]
        [Trait("Type", "Unit")]
        public void ToHexBytes_AllBytes_ReturnCorrectByte()
        {
            for (var x = 0; x < 256; ++x)
            {
                Assert.Equal(new byte[] { (byte)x }, x.ToString("x2").ToHexBytes());
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void TryToHexBytes_Null_ReturnsNull() => Assert.Null(((string)null!).TryToHexBytes());

        [Fact]
        [Trait("Type", "Unit")]
        public void TryToHexBytes_OddLengthString_ReturnsNull() => Assert.Null("123".TryToHexBytes());

        [Theory]
        [InlineData("z1")]
        [InlineData("1z")]
        [InlineData(" 0")]
        [Trait("Type", "Unit")]
        public void TryToHexBytes_BadString_ReturnsNull(string str) => Assert.Null(str.TryToHexBytes());

        [Fact]
        [Trait("Type", "Unit")]
        public void TryToHexBytes_AllBytes_ReturnCorrectByte()
        {
            for (var x = 0; x < 256; ++x)
            {
                Assert.Equal(new byte[] { (byte)x }, x.ToString("x2").TryToHexBytes());
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToHexBytes_SpanNull_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => ((string)null!).ToHexBytes(new byte[0]));

        [Fact]
        [Trait("Type", "Unit")]
        public void ToHexBytes_SpanOddLengthString_ThrowsArgumentOutOfRangeException() => Assert.Throws<ArgumentOutOfRangeException>(() => "123".ToHexBytes(new byte[2]));

        [Theory]
        [InlineData("z1")]
        [InlineData("1z")]
        [InlineData(" 0")]
        [Trait("Type", "Unit")]
        public void ToHexBytes_SpanBadString_ThrowsArgumentOutOfRangeException(string str) => Assert.Throws<ArgumentOutOfRangeException>(() => str.ToHexBytes(new byte[1]));

        [Fact]
        [Trait("Type", "Unit")]
        public void ToHexBytes_SpanAllBytes_ReturnCorrectByte()
        {
            var data = new byte[1];
            for (var x = 0; x < 256; ++x)
            {
                x.ToString("x2").ToHexBytes(data);
                Assert.Equal(x, data[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void NullIfEmpty_Null_ReturnsNull() => Assert.Null(((string)null!).NullIfEmpty());

        [Fact]
        [Trait("Type", "Unit")]
        public void NullIfEmpty_Empty_ReturnsNull() => Assert.Null("".NullIfEmpty());

        [Fact]
        [Trait("Type", "Unit")]
        public void NullIfEmpty_Whitespace_ReturnsString() => Assert.Equal(" ", " ".NullIfEmpty());

        [Fact]
        [Trait("Type", "Unit")]
        public void NullIfEmpty_String_ReturnsString() => Assert.Equal("xxx", "xxx".NullIfEmpty());

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_AllBytesSpan_SetCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[2];
                new byte[] { (byte)x }.AsSpan().ToLowerHexUtf8(target.AsSpan());
                Assert.Equal(new string(target.Select(x => (char)x).ToArray()), x.ToString("x2"));
            }
        }
        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_AllBytesReadOnlySpan_SetCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[2];
                ((ReadOnlySpan<byte>)new byte[] { (byte)x }.AsSpan()).ToLowerHexUtf8(target.AsSpan());
                Assert.Equal(new string(target.Select(x => (char)x).ToArray()), x.ToString("x2"));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_AllBytesSpan_SetCorrectChars()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new char[2];
                new byte[] { (byte)x }.AsSpan().ToLowerHexUtf8(target.AsSpan());
                Assert.Equal(new string(target), x.ToString("x2"));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_AllBytesReadOnlySpan_SetCorrectChars()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new char[2];
                ((ReadOnlySpan<byte>)new byte[] { (byte)x }.AsSpan()).ToLowerHexUtf8(target.AsSpan());
                Assert.Equal(new string(target), x.ToString("x2"));
            }
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(10, 10)]
        [InlineData(10, 19)]
        [InlineData(10, 21)]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_WrongSizeBytes_Throws(int byteSize, int bufferSize) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToLowerHexUtf8(new byte[bufferSize]));

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(10, 10)]
        [InlineData(10, 19)]
        [InlineData(10, 21)]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_WrongSizeChars_Throws(int byteSize, int bufferSize) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToLowerHexUtf8(new char[bufferSize]));


        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_ZeroBytes_DoesntThrow() => new byte[0].AsSpan().ToLowerHexUtf8(new byte[0]);

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_ZeroChars_DoesntThrow() => new byte[0].AsSpan().ToLowerHexUtf8(new char[0]);

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_RandomBytes_MatchesToLowerHex()
        {
            var data = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024);
            var buf = new byte[data.Length * 2];
            data.AsSpan().ToLowerHexUtf8(buf);
            Assert.Equal(new string(buf.Select(x => (char)x).ToArray()), data.ToLowerHex());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToLowerHexUtf8_RandomChars_MatchesToLowerHex()
        {
            var data = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024);
            var buf = new char[data.Length * 2];
            data.AsSpan().ToLowerHexUtf8(buf);
            Assert.Equal(new string(buf), data.ToLowerHex());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllBytesSpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
                var target = new byte[1];
                data.AsSpan().ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllBytesReadOnlySpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
                var target = new byte[1];
                ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllCharsSpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
                var target = new char[1];
                data.AsSpan().ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllCharsReadOnlySpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
                var target = new char[1];
                ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllBytesUppercaseSpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
                var target = new byte[1];
                data.AsSpan().ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllBytesUppercaseReadOnlySpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
                var target = new byte[1];
                ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllCharsUppercaseSpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
                var target = new char[1];
                data.AsSpan().ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllCharsUppercaseReadOnlySpan_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var data = new byte[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray();
                var target = new char[1];
                ((ReadOnlySpan<byte>)data.AsSpan()).ToBytesFromHexUtf8(target);
                Assert.Equal(x, target[0]);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_ZeroBytesSpan_DoesntThrow() => new byte[0].AsSpan().ToBytesFromHexUtf8(new byte[0]);

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_ZeroCharsSpan_DoesntThrow() => new byte[0].AsSpan().ToBytesFromHexUtf8(new char[0]);

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_ZeroBytesReadOnlySpan_DoesntThrow() => ((ReadOnlySpan<byte>)new byte[0].AsSpan()).ToBytesFromHexUtf8(new byte[0]);

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_ZeroCharsReadOnlySpan_DoesntThrow() => ((ReadOnlySpan<byte>)new byte[0].AsSpan()).ToBytesFromHexUtf8(new char[0]);

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(10, 10)]
        [InlineData(10, 19)]
        [InlineData(10, 21)]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_WrongSizeBytesSpan_Throws(int bufferSize, int byteSize) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToBytesFromHexUtf8(new byte[bufferSize]));

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(10, 10)]
        [InlineData(10, 19)]
        [InlineData(10, 21)]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_WrongSizeBytesReadOnlySpan_Throws(int bufferSize, int byteSize) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ReadOnlySpan<byte>)new byte[byteSize].AsSpan()).ToBytesFromHexUtf8(new byte[bufferSize]));

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(10, 10)]
        [InlineData(10, 19)]
        [InlineData(10, 21)]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_WrongSizeCharsSpan_Throws(int bufferSize, int byteSize) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new byte[byteSize].AsSpan().ToBytesFromHexUtf8(new char[bufferSize]));

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(10, 10)]
        [InlineData(10, 19)]
        [InlineData(10, 21)]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_WrongSizeCharsReadOnlySpan_Throws(int bufferSize, int byteSize) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ReadOnlySpan<byte>)new byte[byteSize].AsSpan()).ToBytesFromHexUtf8(new char[bufferSize]));

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_InvalidHexBytesSpan_Throws()
        {
            for (var x = 0; x < 256; ++x)
            {
                if (x >= '0' && x <= '9')
                {
                    continue;
                }
                if (x >= 'a' && x <= 'f')
                {
                    continue;
                }
                if (x >= 'A' && x <= 'F')
                {
                    continue;
                }

                Assert.Throws<FormatException>(() => new byte[] { (byte)0, (byte)x }.AsSpan().ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)x }.AsSpan().ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)0 }.AsSpan().ToBytesFromHexUtf8(new byte[1]));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_InvalidHexBytesReadOnlySpan_Throws()
        {
            for (var x = 0; x < 256; ++x)
            {
                if (x >= '0' && x <= '9')
                {
                    continue;
                }
                if (x >= 'a' && x <= 'f')
                {
                    continue;
                }
                if (x >= 'A' && x <= 'F')
                {
                    continue;
                }

                Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)new byte[] { (byte)0, (byte)x }.AsSpan()).ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)new byte[] { (byte)x, (byte)x }.AsSpan()).ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)new byte[] { (byte)x, (byte)0 }.AsSpan()).ToBytesFromHexUtf8(new byte[1]));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_InvalidHexCharsSpan_Throws()
        {
            for (var x = 0; x < 256; ++x)
            {
                if (x >= '0' && x <= '9')
                {
                    continue;
                }
                if (x >= 'a' && x <= 'f')
                {
                    continue;
                }
                if (x >= 'A' && x <= 'F')
                {
                    continue;
                }

                Assert.Throws<FormatException>(() => new byte[] { (byte)0, (byte)x }.AsSpan().ToBytesFromHexUtf8(new char[1]));
                Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)x }.AsSpan().ToBytesFromHexUtf8(new char[1]));
                Assert.Throws<FormatException>(() => new byte[] { (byte)x, (byte)0 }.AsSpan().ToBytesFromHexUtf8(new char[1]));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_InvalidHexCharsReadOnlySpan_Throws()
        {
            for (var x = 0; x < 256; ++x)
            {
                if (x >= '0' && x <= '9')
                {
                    continue;
                }
                if (x >= 'a' && x <= 'f')
                {
                    continue;
                }
                if (x >= 'A' && x <= 'F')
                {
                    continue;
                }

                Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)new byte[] { (byte)0, (byte)x }.AsSpan()).ToBytesFromHexUtf8(new char[1]));
                Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)new byte[] { (byte)x, (byte)x }.AsSpan()).ToBytesFromHexUtf8(new char[1]));
                Assert.Throws<FormatException>(() => ((ReadOnlySpan<byte>)new byte[] { (byte)x, (byte)0 }.AsSpan()).ToBytesFromHexUtf8(new char[1]));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_EmptyBytesSequence_DoesntThrow() => ReadOnlySequence<byte>.Empty.ToBytesFromHexUtf8(new byte[0]);
        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_EmptyCharsSequence_DoesntThrow() => ReadOnlySequence<char>.Empty.ToBytesFromHexUtf8(new byte[0]);

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllByteSequences_SingleContiguous_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[1];
                new ReadOnlySequence<byte>(new[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray()).ToBytesFromHexUtf8(target);
                Assert.Equal(target[0], x);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllCharsSequences_SingleContiguous_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[1];
                new ReadOnlySequence<char>(new[] { (byte)x }.ToLowerHex().ToArray()).ToBytesFromHexUtf8(target);
                Assert.Equal(target[0], x);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllByteSequences_SingleContiguousUppercase_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[1];
                new ReadOnlySequence<byte>(new[] { (byte)x }.ToLowerHex().ToUpper().Select(x => (byte)x).ToArray()).ToBytesFromHexUtf8(target);
                Assert.Equal(target[0], x);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllCharsSequences_SingleContiguousUppercase_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[1];
                new ReadOnlySequence<char>(new[] { (byte)x }.ToLowerHex().ToUpper().ToArray()).ToBytesFromHexUtf8(target);
                Assert.Equal(target[0], x);
            }
        }

        class ByteSeq : ReadOnlySequenceSegment<byte>
        {
            public ByteSeq(ByteSeq prev, byte data) : this(prev, new[] { data }) { }
            public ByteSeq(ByteSeq prev, byte[] data)
            {
                if (prev != null)
                {
                    prev.Next = this;
                    RunningIndex = prev.RunningIndex + prev.Memory.Length;
                }
                Memory = data;
            }
        }
        class CharSeq : ReadOnlySequenceSegment<char>
        {
            public CharSeq(CharSeq prev, char data) : this(prev, new[] { data }) { }
            public CharSeq(CharSeq prev, char[] data)
            {
                if (prev != null)
                {
                    prev.Next = this;
                    RunningIndex = prev.RunningIndex + prev.Memory.Length;
                }
                Memory = data;
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllByteSequences_SingleSplit_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[1];
                var hex = new[] { (byte)x }.ToLowerHex().Select(x => (byte)x).ToArray();
                var seq1 = new ByteSeq(null!, hex[0]);
                var seq2 = new ByteSeq(seq1, hex[1]);
                var seq = new ReadOnlySequence<byte>(seq1, 0, seq2, 1);
                seq.ToBytesFromHexUtf8(target);
                Assert.Equal(target[0], x);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_AllCharSequences_SingleSplit_SetsCorrectBytes()
        {
            for (var x = 0; x < 256; ++x)
            {
                var target = new byte[1];
                var hex = new[] { (byte)x }.ToLowerHex().ToArray();
                var seq1 = new CharSeq(null!, hex[0]);
                var seq2 = new CharSeq(seq1, hex[1]);
                var seq = new ReadOnlySequence<char>(seq1, 0, seq2, 1);
                seq.ToBytesFromHexUtf8(target);
                Assert.Equal(target[0], x);
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_InvalidHexBytesSequence_Throws()
        {
            for (var x = 0; x < 256; ++x)
            {
                if (x >= '0' && x <= '9')
                {
                    continue;
                }
                if (x >= 'a' && x <= 'f')
                {
                    continue;
                }
                if (x >= 'A' && x <= 'F')
                {
                    continue;
                }

                Assert.Throws<FormatException>(() => new ReadOnlySequence<byte>(new byte[] { (byte)0, (byte)x }).ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => new ReadOnlySequence<byte>(new byte[] { (byte)x, (byte)x }).ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => new ReadOnlySequence<byte>(new byte[] { (byte)x, (byte)0 }).ToBytesFromHexUtf8(new byte[1]));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_InvalidHexCharsSequence_Throws()
        {
            for (var x = 0; x < 256; ++x)
            {
                if (x >= '0' && x <= '9')
                {
                    continue;
                }
                if (x >= 'a' && x <= 'f')
                {
                    continue;
                }
                if (x >= 'A' && x <= 'F')
                {
                    continue;
                }

                Assert.Throws<FormatException>(() => new ReadOnlySequence<char>(new byte[] { (byte)0, (byte)x }.Select(x => (char)x).ToArray()).ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => new ReadOnlySequence<char>(new byte[] { (byte)x, (byte)x }.Select(x => (char)x).ToArray()).ToBytesFromHexUtf8(new byte[1]));
                Assert.Throws<FormatException>(() => new ReadOnlySequence<char>(new byte[] { (byte)x, (byte)0 }.Select(x => (char)x).ToArray()).ToBytesFromHexUtf8(new byte[1]));
            }
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_RandomBytesSingle_MatchesToHexBytes()
        {
            var hexstr = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
            var buf = new byte[hexstr.Length / 2];
            new ReadOnlySequence<byte>(hexstr.Select(x => (byte)x).ToArray()).ToBytesFromHexUtf8(buf);
            Assert.Equal(buf, hexstr.ToHexBytes());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_RandonCharsSingle_MatchesToHexBytes()
        {
            var hexstr = RandomHelper.GetRandomBytes(RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
            var buf = new byte[hexstr.Length / 2];
            new ReadOnlySequence<char>(hexstr.ToArray()).ToBytesFromHexUtf8(buf);
            Assert.Equal(buf, hexstr.ToHexBytes());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_RandomBytesSplit_MatchesToHexBytes()
        {
            var hexstr = RandomHelper.GetRandomBytes(2 + RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
            var buf = new byte[hexstr.Length / 2];
            var split = hexstr.Length / 2 + RandomHelper.GetRandomInt32() % 2;
            var seq1 = new ByteSeq(null!, hexstr.Substring(0, split).Select(x => (byte)x).ToArray());
            var seq2 = new ByteSeq(seq1, hexstr.Substring(split).Select(x => (byte)x).ToArray());
            new ReadOnlySequence<byte>(seq1, 0, seq2, seq2.Memory.Length).ToBytesFromHexUtf8(buf);
            Assert.Equal(buf, hexstr.ToHexBytes());
        }

        [Fact]
        [Trait("Type", "Unit")]
        public void ToBytesFromHexUtf8_RandomCharsSplit_MatchesToHexBytes()
        {
            var hexstr = RandomHelper.GetRandomBytes(2 + RandomHelper.GetRandomPositiveInt32() % 1024).ToLowerHex();
            var buf = new byte[hexstr.Length / 2];
            var split = hexstr.Length / 2 + RandomHelper.GetRandomInt32() % 2;
            var seq1 = new CharSeq(null!, hexstr.Substring(0, split).ToArray());
            var seq2 = new CharSeq(seq1, hexstr.Substring(split).ToArray());
            new ReadOnlySequence<char>(seq1, 0, seq2, seq2.Memory.Length).ToBytesFromHexUtf8(buf);
            Assert.Equal(buf, hexstr.ToHexBytes());
        }

        [Theory]
        [InlineData("")]
        [InlineData("03")]
        [InlineData("3f")]
        [InlineData("f83eb1")]
        [InlineData("ffff0000abcd7632")]
        public void ToLowerHex_ByteSpan_TestCases(string str) => Assert.Equal(str, CrappyToBytes(str).AsSpan().ToLowerHex());

        [Theory]
        [InlineData("")]
        [InlineData("03")]
        [InlineData("3f")]
        [InlineData("f83eb1")]
        [InlineData("ffff0000abcd7632")]
        public void ToLowerHex_ByteReadOnlySpan_TestCases(string str) => Assert.Equal(str, ((ReadOnlySpan<byte>)CrappyToBytes(str).AsSpan()).ToLowerHex());

        [Theory]
        [InlineData("")]
        [InlineData("03")]
        [InlineData("3f")]
        [InlineData("f83eb1")]
        [InlineData("ffff0000abcd7632")]
        public void ToBytesFromHexUtf8_ByteSpan_TestCases(string str)
        {
            var expected = CrappyToBytes(str);
            var data = new byte[expected.Length];
            str.Select(x => (byte)x).ToArray().AsSpan().ToBytesFromHexUtf8(data);
            Assert.Equal(expected, data);
        }

        [Theory]
        [InlineData("")]
        [InlineData("03")]
        [InlineData("3f")]
        [InlineData("f83eb1")]
        [InlineData("ffff0000abcd7632")]
        public void ToBytesFromHexUtf8_ByteReadOnlySpan_TestCases(string str)
        {
            var expected = CrappyToBytes(str);
            var data = new byte[expected.Length];
            ((ReadOnlySpan<byte>)str.Select(x => (byte)x).ToArray().AsSpan()).ToBytesFromHexUtf8(data);
            Assert.Equal(expected, data);
        }

        [Theory]
        [InlineData("")]
        [InlineData("03")]
        [InlineData("3f")]
        [InlineData("f83eb1")]
        [InlineData("ffff0000abcd7632")]
        public void ToBytesFromHexUtf8_CharSpan_TestCases(string str)
        {
            var expected = CrappyToBytes(str);
            var data = new char[expected.Length];
            str.Select(x => (byte)x).ToArray().AsSpan().ToBytesFromHexUtf8(data);
            Assert.Equal(expected.Select(x => (char)x), data);
        }

        [Theory]
        [InlineData("")]
        [InlineData("03")]
        [InlineData("3f")]
        [InlineData("f83eb1")]
        [InlineData("ffff0000abcd7632")]
        public void ToBytesFromHexUtf8_CharReadOnlySpan_TestCases(string str)
        {
            var expected = CrappyToBytes(str);
            var data = new char[expected.Length];
            ((ReadOnlySpan<byte>)str.Select(x => (byte)x).ToArray().AsSpan()).ToBytesFromHexUtf8(data);
            Assert.Equal(expected.Select(x => (char)x), data);
        }
    }
}
