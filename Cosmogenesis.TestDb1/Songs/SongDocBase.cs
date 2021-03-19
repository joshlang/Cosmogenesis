using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb1.Songs
{
    [Partition("Songs")]
    public abstract class SongDocBase : DbDoc
    {
        [PartitionDefinition("Songs")]
        public static string GetPk(string name) => $"Song: {name}";

        public string Name { get; set; } = default!;
    }
}
