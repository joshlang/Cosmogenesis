namespace Cosmogenesis.TestDb1.Singers
{
    public sealed class SeaShantySingerDoc : SingerDocBase
    {
        public static string GetId(string firstName, string lastName) => $"SeaShanty: {firstName} {lastName}";
    }
}
