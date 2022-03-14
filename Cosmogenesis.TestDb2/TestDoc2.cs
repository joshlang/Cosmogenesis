namespace Cosmogenesis.TestDb2;

[Db("TestDb2_2", Namespace = "Other.Namespace")]
[Partition("JustOnePartition")]
public class TestDoc2 : DbDoc
{
    public static string GetPk(string thing) => $"thing={thing}";
    public static string GetId() => "SingletonThing";

    public string Thing { get; init; } = default!;

    public int Count { get; init; }
}
