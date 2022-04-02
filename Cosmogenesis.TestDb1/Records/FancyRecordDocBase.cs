namespace Cosmogenesis.TestDb1.Records;
public abstract class FancyRecordDocBase : RecordDocBase
{
    public static string GetId(int fancinessLevel) => $"Fanciness={fancinessLevel}";

    public int FancinessLevel { get; init; }
}
