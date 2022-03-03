namespace Cosmogenesis.TestDb2;

[Db("TestDb2_1")]
[Partition("TheOnlyPartition")]
public class TestDoc1 : DbDoc
{
    public static string GetPk(string thing) => $"thing={thing}";
    public static string GetId() => "SingletonThing";

    public string Thing { get; set; } = default!;

    public int Count { get; set; }
}
