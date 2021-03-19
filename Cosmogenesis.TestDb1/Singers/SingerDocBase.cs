using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb1.Singers
{
    [Partition("Singers")]
    public abstract class SingerDocBase: DbDoc
    {
        [PartitionDefinition("Singers")]
        public static string GetPartitionKey() => "Singers";

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        [UseDefault]
        public string? MiddleName { get; set; }
    }
}
