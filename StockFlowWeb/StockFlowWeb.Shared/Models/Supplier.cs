using SQLite;

namespace StockFlowWeb.Shared.Models;

public class Supplier
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ContactEmail { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}