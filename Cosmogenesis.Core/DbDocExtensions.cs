using System;

namespace Cosmogenesis.Core
{
    public static class DbDocExtensions
    {
        public static DateTime GetApproxLastChangeDate(this DbDoc dbDoc) => DateTimeOffset.FromUnixTimeSeconds(dbDoc._ts).UtcDateTime;
    }
}
