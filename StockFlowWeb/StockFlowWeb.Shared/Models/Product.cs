using SQLite;

namespace StockFlowWeb.Shared.Models;

public class Product
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string SKU { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public int Quantity { get; set; }
    public int ReorderPoint { get; set; }
    public int ReorderQty { get; set; }
    public decimal UnitPrice { get; set; }
    public string ImagePath { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Ignore]
    public decimal TotalValue => Quantity * UnitPrice;

    [Ignore]
    public bool IsLowStock => Quantity <= ReorderPoint;

    [Ignore]
    public string StockStatus => Quantity == 0 ? "Out of Stock"
        : Quantity <= ReorderPoint ? "Low Stock"
        : "In Stock";
}