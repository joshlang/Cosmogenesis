using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb1.Singers
{
    [Transient]
    public sealed class NotAsGoodAsSeaShantySingerDoc : SingerDocBase
    {
        [DocumentId]
        public static string GetIdButNamedSomethingElse(string firstName, string lastName) => $"NotSeaShanty: {firstName} {lastName}";
    }
}
