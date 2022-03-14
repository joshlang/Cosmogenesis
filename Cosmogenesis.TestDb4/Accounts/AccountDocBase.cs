using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb4.Accounts;

[Partition("Accounts")]
public abstract class AccountDocBase : DbDoc
{
    public static string GetPk(Guid accountId) => $"Account={accountId:N}";

    public Guid AccountId { get; init; }
}
