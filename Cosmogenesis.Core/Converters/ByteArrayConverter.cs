using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cosmogenesis.Core.Converters
{
    public sealed class ByteArrayConverter : JsonConverter<byte[]>
    {
        public static readonly ByteArrayConverter Instance = new();

        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidOperationException("Only string can be converted to byte[] with this converter");
            }

            if (reader.HasValueSequence)
            {
                var hexSequence = reader.ValueSequence;
                var data = new byte[hexSequence.Length / 2];
                hexSequence.ToBytesFromHexUtf8(data);
                return data;
            }
            else
            {
                var hexSpan = reader.ValueSpan;
                var data = new byte[hexSpan.Length / 2];
                hexSpan.ToBytesFromHexUtf8(data);
                return data;
            }
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            Span<byte> hex =
                value.Length <= 1024
                ? stackalloc byte[value.Length * 2]
                : new byte[value.Length * 2];
            value.AsSpan().ToLowerHexUtf8(hex);
            writer.WriteStringValue(hex);
        }
    }
}
