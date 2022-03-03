using System;

namespace Cosmogenesis.TestDb1.Songs;

[Db("Music", Namespace = "Music")]
public class SongDoc : SongDocBase
{
    public static string GetId() => "Song";

    public string SingerFirstName { get; set; } = default!;
    public string SingerLastName { get; set; } = default!;
    public DateTime PremierDate { get; set; }
}
