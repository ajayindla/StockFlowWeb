using SQLite;

namespace StockFlowWeb.Shared.Models;

public class StockTransaction
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Delta { get; set; }
    public string Reason { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Now;

    [Ignore]
    public string DeltaDisplay => Delta > 0 ? $"+{Delta}" : $"{Delta}";

    [Ignore]
    public string Type => Delta > 0 ? "IN" : "OUT";
}