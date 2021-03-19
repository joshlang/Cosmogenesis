using System;
using System.Collections.Generic;
using System.Text;
using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb2
{
    [Db("TestDb2_2")]
    public class TestDoc2: DbDoc
    {
        [PartitionDefinition]
        public static string JustOnePartition(string thing) => $"thing={thing}";
        public static string GetId() => "SingletonThing";

        public string Thing { get; set; } = default!;

        public int Count { get; set; }
    }
}
