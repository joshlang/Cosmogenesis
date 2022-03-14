using Cosmogenesis.Core;
using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb4.Sales;

[Db("EvilCorp", Namespace = "Evil.Corp.Database")]
[Partition("Orders")]
public sealed class OrderDoc : DbDoc
{
    public static string GetPk(Guid accountId) => $"Orders={accountId:N}";
    public static string GetId(string orderNumber) => $"Order={orderNumber}";

    public Guid AccountId { get; init; }
    public string OrderNumber { get; init; } = default!;

    public class Item
    {
        public string ItemCode { get; set; } = default!;
        public decimal UnitCost { get; set; }
        public long Quantity { get; set; }
    }

    public Item[] Items { get; init; } = default!;
    public List<string> Notes { get; init; } = default!;
    public decimal TotalPrice { get; init; }
}
