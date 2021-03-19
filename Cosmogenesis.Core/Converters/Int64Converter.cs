using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cosmogenesis.Core.Converters
{
    public sealed class Int64Converter : JsonConverter<long>
    {
        public static readonly Int64Converter Instance = new();

        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidOperationException("Only string can be converted to long with this converter");
            }

            if (long.TryParse(reader.GetString(), out var value))
            {
                return value;
            }

            throw new FormatException("The value could not be parsed into a long");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
    }
}
