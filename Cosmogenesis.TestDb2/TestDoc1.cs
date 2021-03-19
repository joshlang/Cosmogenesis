using System;
using System.Collections.Generic;
using System.Text;
using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb2
{
    [Db("TestDb2_1")]
    public class TestDoc1: DbDoc
    {
        [PartitionDefinition]
        public static string TheOnlyPartition(string thing) => $"thing={thing}";
        public static string GetId() => "SingletonThing";

        public string Thing { get; set; } = default!;

        public int Count { get; set; }
    }
}
