namespace Cosmogenesis.TestDb1.Singers;

[Partition("Singers")]
public abstract class SingerDocBase : DbDoc
{
    public static string GetPk() => "Singers";

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    [UseDefault]
    public string? MiddleName { get; set; }
}
