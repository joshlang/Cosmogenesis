using System;
using System.Globalization;
using System.Linq;

namespace Cosmogenesis.Core
{
    /// <summary>
    /// Because we want to use TryParseExact for any of these formats.  
    /// "o" is too strict and there are no other built-in options.
    /// </summary>
    public static class IsoDateCheater
    {
        /// <summary>
        /// DateTime.MinValue, but with Utc date type.
        /// (DateTime.MinValue == IsoDateCheater.MinValue) is true.
        /// </summary>
        public static readonly DateTime MinValue = new DateTime(0, DateTimeKind.Utc);
        /// <summary>
        /// DateTime.MaxValue, but with Utc date type.
        /// (DateTime.MaxValue == IsoDateCheater.MaxValue) is true.
        /// </summary>
        public static readonly DateTime MaxValue = new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);

        static readonly string[] DecimalFormats = new[]
        {
            "yyyy-MM-dd'T'HH:mm:ss'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.f'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.ff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.ffff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fffff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'",
        };
        const int DecimalIndex = 19; // (the quotes don't count!)

        public static string GetFormat(int decimalCount) =>
            decimalCount < 0 || decimalCount >= DecimalFormats.Length
            ? throw new ArgumentOutOfRangeException(nameof(decimalCount), $"0-{DecimalFormats.Length - 1} only")
            : DecimalFormats[decimalCount];

        public static string[] GetFormats() => DecimalFormats.ToArray();

        static bool TryParseExactUtc(string dateString, string format, out DateTime date)
        {
            if (DateTime.TryParseExact(dateString, format, null, DateTimeStyles.RoundtripKind, out date))
            {
                date = new DateTime(date.Ticks, DateTimeKind.Utc);
                return true;
            }
            return false;
        }

        public static bool TryParse(string? dateString, out DateTime date)
        {
            if (dateString?.Length > DecimalIndex &&
                dateString[^1] == 'Z')
            {
                if (dateString[DecimalIndex] == 'Z')
                {
                    return TryParseExactUtc(dateString, DecimalFormats[0], out date);
                }
                if (dateString[DecimalIndex] == '.')
                {
                    var formatIndex = dateString.Length - DecimalIndex - 2;
                    if (formatIndex < DecimalFormats.Length)
                    {
                        return TryParseExactUtc(dateString, DecimalFormats[formatIndex], out date);
                    }
                }
            }
            date = default;
            return false;
        }

        public static DateTime Parse(string dateString) =>
            TryParse(dateString, out var date)
            ? date
            : throw new FormatException($"Iso date should be in a format like {DecimalFormats[^1]}");
    }
}
