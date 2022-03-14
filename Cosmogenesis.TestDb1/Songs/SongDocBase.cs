namespace Cosmogenesis.TestDb1.Songs;

[Partition("Songs")]
public abstract class SongDocBase : DbDoc
{
    public static string GetPk(string name) => $"Song: {name}";

    public string Name { get; init; } = default!;
}
