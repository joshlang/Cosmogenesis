namespace Cosmogenesis.TestDb1.Singers;

[Partition("Singers")]
public abstract class SingerDocBase : DbDoc
{
    public static string GetPk() => "Singers";

    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;

    [UseDefault]
    public string? MiddleName { get; set; }
}
