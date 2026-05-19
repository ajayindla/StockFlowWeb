using SQLite;

namespace StockFlowWeb.Shared.Models;

public class Category
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ColorHex { get; set; } = "#6366f1";
    public string Icon { get; set; } = "📦";
}