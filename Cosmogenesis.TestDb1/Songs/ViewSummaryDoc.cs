namespace Cosmogenesis.TestDb1.Songs
{
    public class ViewSummaryDoc : SongDocBase
    {
        public static string GetId(string platform) => $"ViewSummary: {platform}";

        public string Platform { get; set; } = default!;
        public long ViewCount { get; set; } = default!;
    }
}
