using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb3
{
    [Db("CoolStuff", Namespace = "CoolStuff")]
    [PartitionDefinition]
    static class Partitions
    {
        public static string Apples(string appleType) => $"Apple({appleType})";
        public static string Oranges(string orangeType) => $"Orange({orangeType})";
    }
}
