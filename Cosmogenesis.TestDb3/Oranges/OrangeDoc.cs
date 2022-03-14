using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb3.Oranges;

[Partition("Oranges")]
public class OrangeDoc : DbDoc
{
    internal static string GetId() => "Singleton";
    public string OrangeType { get; init; } = default!;
    public int Count { get; init; }
    public bool IsBetterThanApples { get; init; }
}
