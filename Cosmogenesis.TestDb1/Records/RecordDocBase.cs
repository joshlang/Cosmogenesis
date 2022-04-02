namespace Cosmogenesis.TestDb1.Records;

[Partition("Record")]
public abstract class RecordDocBase : DbDoc
{
    public static string GetPk() => "Records";
}
