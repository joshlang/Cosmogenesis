using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb3.Apples;

[Partition("Apples")]
public class AppleDoc : DbDoc
{
    internal static string GetId() => "Singleton";
    public string AppleType { get; init; } = default!;
    public int Count { get; init; }
}
