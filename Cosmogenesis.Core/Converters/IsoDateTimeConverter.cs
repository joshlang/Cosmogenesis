using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cosmogenesis.Core.Converters
{
    /// <summary>
    /// Always outputs 7 digits of fractional seconds (ISO 8601).
    /// But can read 0-7 fractional seconds (as per IsoDateCheater)
    /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
    /// Note: Not as flexible as https://docs.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support#the-extended-iso-8601-12019-profile-in-systemtextjson which takes 8+ fractional seconds and potentially other formats.
    /// </summary>
    public sealed class IsoDateTimeConverter : JsonConverter<DateTime>
    {
        public static readonly IsoDateTimeConverter Instance = new();

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidOperationException("Only string can be converted to DateTime with this converter");
            }

            if (IsoDateCheater.TryParse(reader.GetString(), out var value))
            {
                return value;
            }

            throw new FormatException("The value could not be parsed into a DateTime");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                throw new InvalidOperationException($"Only UTC dates can be serialized with this converter");
            }

            writer.WriteStringValue(value.ToString("O"));
        }
    }
}
