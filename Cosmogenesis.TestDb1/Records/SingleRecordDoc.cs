namespace Cosmogenesis.TestDb1.Records;
public sealed class SingleRecordDoc : RecordDocBase
{
    public static string GetId(string name) => $"Record={name}";

    public string Name { get; init; } = default!;
}
