using SQLite;

namespace StockFlowWeb.Shared.Models;

public class PurchaseOrderItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }

    [Ignore]
    public decimal LineTotal => Quantity * UnitCost;
}