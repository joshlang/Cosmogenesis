using System;
using System.Collections.Generic;
using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb4.Sales
{
    [Partition("Orders")]
    public sealed class OrderDoc : DbDoc
    {
        [PartitionDefinition("Orders")]
        public static string GetPk(Guid accountId) => $"Orders={accountId:N}";
        public static string GetId(string orderNumber) => $"Order={orderNumber}";

        public Guid AccountId { get; set; }
        public string OrderNumber { get; set; } = default!;

        public class Item
        {
            public string ItemCode { get; set; } = default!;
            public decimal UnitCost { get; set; }
            public long Quantity { get; set; }
        }

        public Item[] Items { get; set; } = default!;
        public List<string> Notes { get; set; } = default!;
        public decimal TotalPrice { get; set; }
    }
}
