using StockFlowWeb.Shared.Models;

namespace StockFlowWeb.Shared.Services;

public class CsvExportService
{
    public string ExportProducts(List<Product> products)
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("SKU,Name,Description,Quantity,Reorder Point,Reorder Qty,Unit Price,Total Value,Stock Status");
        foreach (var p in products)
            csv.AppendLine($"{Escape(p.SKU)},{Escape(p.Name)},{Escape(p.Description)},{p.Quantity},{p.ReorderPoint},{p.ReorderQty},{p.UnitPrice:F2},{p.TotalValue:F2},{Escape(p.StockStatus)}");
        return csv.ToString();
    }

    public string ExportTransactions(List<StockTransaction> transactions)
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Date,Product ID,Type,Delta,Reason,Notes");
        foreach (var t in transactions)
            csv.AppendLine($"{t.Date:yyyy-MM-dd},{t.ProductId},{Escape(t.Type)},{t.Delta},{Escape(t.Reason)},{Escape(t.Notes)}");
        return csv.ToString();
    }

    public string ExportSuppliers(List<Supplier> suppliers)
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Name,Email,Phone,Address,Created At");
        foreach (var s in suppliers)
            csv.AppendLine($"{Escape(s.Name)},{Escape(s.ContactEmail)},{Escape(s.Phone)},{Escape(s.Address)},{s.CreatedAt:yyyy-MM-dd}");
        return csv.ToString();
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}