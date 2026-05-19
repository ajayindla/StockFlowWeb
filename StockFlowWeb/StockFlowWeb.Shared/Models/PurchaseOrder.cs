using SQLite;

namespace StockFlowWeb.Shared.Models;

public class PurchaseOrder
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? ReceivedDate { get; set; }
    public string Notes { get; set; } = "";

    [Ignore]
    public string OrderNumber => $"PO-{Id:D4}";
}