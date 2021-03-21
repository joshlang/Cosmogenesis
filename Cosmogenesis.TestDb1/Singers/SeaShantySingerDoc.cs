using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb1.Singers
{
    [DocType("SeaShantySinger")]
    public sealed class SeaShantySingerDoc : SingerDocBase
    {
        public static string GetId(string firstName, string lastName) => $"SeaShanty: {firstName} {lastName}";
    }
}
