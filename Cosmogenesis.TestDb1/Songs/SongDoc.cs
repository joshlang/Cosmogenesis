using System;

namespace Cosmogenesis.TestDb1.Songs;

public class SongDoc : SongDocBase
{
    public static string GetId() => "Song";

    public string SingerFirstName { get; init; } = default!;
    public string SingerLastName { get; init; } = default!;
    public DateTime PremierDate { get; init; }
}
