using System;
using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb4.Accounts
{
    [Partition("Accounts")]
    public abstract class AccountDocBase : DbDoc
    {
        [PartitionDefinition("Accounts")]
        public static string GetPk(Guid accountId) => $"Account={accountId:N}";

        public Guid AccountId { get; set; }
    }
}
