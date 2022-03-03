using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb4.Accounts;

[Mutable]
public sealed class AccountInfoDoc : AccountDocBase
{
    public static string GetId() => "Info";

    public string Name { get; set; } = default!;
    public bool IsEvil { get; set; }
    public int MinionCount { get; set; }
}
