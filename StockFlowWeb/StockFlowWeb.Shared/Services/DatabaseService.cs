using SQLite;
using StockFlowWeb.Shared.Models;

namespace StockFlowWeb.Shared.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;
    private bool _initialized = false;
    private readonly IDatabasePath _pathProvider;

    public DatabaseService(IDatabasePath pathProvider)
    {
        _pathProvider = pathProvider;
    }

    public async Task InitAsync()
    {
        if (_initialized) return;

        var dbPath = _pathProvider.GetPath();
        _db = new SQLiteAsyncConnection(dbPath);

        await _db.CreateTableAsync<Category>();
        await _db.CreateTableAsync<Supplier>();
        await _db.CreateTableAsync<Product>();
        await _db.CreateTableAsync<StockTransaction>();
        await _db.CreateTableAsync<PurchaseOrder>();
        await _db.CreateTableAsync<PurchaseOrderItem>();

        var count = await _db.Table<Product>().CountAsync();
        if (count == 0) await SeedDataAsync();

        _initialized = true;
    }

    private async Task EnsureInit() => await InitAsync();

    // ── Categories ────────────────────────────────────────────
    public async Task<List<Category>> GetCategoriesAsync()
    {
        await EnsureInit();
        return await _db!.Table<Category>().ToListAsync();
    }

    public async Task<int> SaveCategoryAsync(Category category)
    {
        await EnsureInit();
        return category.Id == 0
            ? await _db!.InsertAsync(category)
            : await _db!.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        await EnsureInit();
        await _db!.DeleteAsync(category);
    }

    // ── Suppliers ─────────────────────────────────────────────
    public async Task<List<Supplier>> GetSuppliersAsync()
    {
        await EnsureInit();
        return await _db!.Table<Supplier>().ToListAsync();
    }

    public async Task<int> SaveSupplierAsync(Supplier supplier)
    {
        await EnsureInit();
        return supplier.Id == 0
            ? await _db!.InsertAsync(supplier)
            : await _db!.UpdateAsync(supplier);
    }

    public async Task DeleteSupplierAsync(Supplier supplier)
    {
        await EnsureInit();
        await _db!.DeleteAsync(supplier);
    }

    // ── Products ──────────────────────────────────────────────
    public async Task<List<Product>> GetProductsAsync()
    {
        await EnsureInit();
        return await _db!.Table<Product>().ToListAsync();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        await EnsureInit();
        return await _db!.Table<Product>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetLowStockProductsAsync()
    {
        await EnsureInit();
        var all = await _db!.Table<Product>().ToListAsync();
        return all.Where(p => p.Quantity <= p.ReorderPoint).ToList();
    }

    public async Task<List<Product>> SearchProductsAsync(string query)
    {
        await EnsureInit();
        var lower = query.ToLower();
        var all = await _db!.Table<Product>().ToListAsync();
        return all.Where(p =>
            p.Name.ToLower().Contains(lower) ||
            p.SKU.ToLower().Contains(lower) ||
            p.Description.ToLower().Contains(lower)).ToList();
    }

    public async Task<int> SaveProductAsync(Product product)
    {
        await EnsureInit();
        product.UpdatedAt = DateTime.Now;
        if (product.Id == 0)
        {
            product.CreatedAt = DateTime.Now;
            return await _db!.InsertAsync(product);
        }
        return await _db!.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(Product product)
    {
        await EnsureInit();
        await _db!.DeleteAsync(product);
    }

    // ── Stock Transactions ────────────────────────────────────
    public async Task<List<StockTransaction>> GetTransactionsAsync(int productId)
    {
        await EnsureInit();
        return await _db!.Table<StockTransaction>()
            .Where(t => t.ProductId == productId).ToListAsync();
    }

    public async Task<List<StockTransaction>> GetRecentTransactionsAsync(int count = 20)
    {
        await EnsureInit();
        var all = await _db!.Table<StockTransaction>().ToListAsync();
        return all.OrderByDescending(t => t.Date).Take(count).ToList();
    }

    public async Task AdjustStockAsync(int productId, int delta, string reason, string notes = "")
    {
        await EnsureInit();
        var product = await GetProductAsync(productId);
        if (product == null) return;

        product.Quantity = Math.Max(0, product.Quantity + delta);
        product.UpdatedAt = DateTime.Now;
        await _db!.UpdateAsync(product);

        await _db!.InsertAsync(new StockTransaction
        {
            ProductId = productId,
            Delta = delta,
            Reason = reason,
            Notes = notes,
            Date = DateTime.Now
        });
    }

    // ── Purchase Orders ───────────────────────────────────────
    public async Task<List<PurchaseOrder>> GetPurchaseOrdersAsync()
    {
        await EnsureInit();
        var all = await _db!.Table<PurchaseOrder>().ToListAsync();
        return all.OrderByDescending(o => o.OrderDate).ToList();
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderAsync(int id)
    {
        await EnsureInit();
        return await _db!.Table<PurchaseOrder>().Where(o => o.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SavePurchaseOrderAsync(PurchaseOrder order)
    {
        await EnsureInit();
        return order.Id == 0
            ? await _db!.InsertAsync(order)
            : await _db!.UpdateAsync(order);
    }

    public async Task ReceivePurchaseOrderAsync(int orderId)
    {
        await EnsureInit();
        var order = await GetPurchaseOrderAsync(orderId);
        if (order == null) return;

        var items = await _db!.Table<PurchaseOrderItem>()
            .Where(i => i.OrderId == orderId).ToListAsync();

        foreach (var item in items)
            await AdjustStockAsync(item.ProductId, item.Quantity,
                "Purchase Order", $"PO-{orderId:D4} received");

        order.Status = "Received";
        order.ReceivedDate = DateTime.Now;
        await _db!.UpdateAsync(order);
    }

    // ── Purchase Order Items ──────────────────────────────────
    public async Task<List<PurchaseOrderItem>> GetOrderItemsAsync(int orderId)
    {
        await EnsureInit();
        return await _db!.Table<PurchaseOrderItem>()
            .Where(i => i.OrderId == orderId).ToListAsync();
    }

    public async Task<int> SaveOrderItemAsync(PurchaseOrderItem item)
    {
        await EnsureInit();
        return item.Id == 0
            ? await _db!.InsertAsync(item)
            : await _db!.UpdateAsync(item);
    }

    // ── Dashboard Stats ───────────────────────────────────────
    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        await EnsureInit();
        var products = await _db!.Table<Product>().ToListAsync();
        var orders = await _db!.Table<PurchaseOrder>().ToListAsync();
        var transactions = await GetRecentTransactionsAsync(10);

        return new DashboardStats
        {
            TotalProducts = products.Count,
            LowStockCount = products.Count(p => p.Quantity <= p.ReorderPoint),
            OutOfStockCount = products.Count(p => p.Quantity == 0),
            TotalInventoryValue = products.Sum(p => p.Quantity * p.UnitPrice),
            PendingOrders = orders.Count(o => o.Status == "Pending" || o.Status == "Ordered"),
            RecentTransactions = transactions
        };
    }

    // ── Seed Data ─────────────────────────────────────────────
    private async Task SeedDataAsync()
    {
        var categories = new List<Category>
        {
            new() { Name = "Electronics",     ColorHex = "#6366f1", Icon = "📱" },
            new() { Name = "Clothing",        ColorHex = "#ec4899", Icon = "👕" },
            new() { Name = "Food & Beverage", ColorHex = "#f59e0b", Icon = "🍎" },
            new() { Name = "Tools",           ColorHex = "#10b981", Icon = "🔧" },
            new() { Name = "Office Supplies", ColorHex = "#3b82f6", Icon = "📎" },
        };
        foreach (var c in categories) await _db!.InsertAsync(c);

        var suppliers = new List<Supplier>
        {
            new() { Name = "TechSource Inc.",    ContactEmail = "orders@techsource.com",    Phone = "555-0101", Address = "123 Tech Blvd, Austin TX" },
            new() { Name = "Global Apparel Co.", ContactEmail = "supply@globalapparel.com", Phone = "555-0102", Address = "456 Fashion Ave, New York NY" },
            new() { Name = "FreshFoods Ltd.",    ContactEmail = "orders@freshfoods.com",    Phone = "555-0103", Address = "789 Market St, Chicago IL" },
            new() { Name = "ProTools Supply",    ContactEmail = "sales@protools.com",        Phone = "555-0104", Address = "321 Industrial Rd, Houston TX" },
        };
        foreach (var s in suppliers) await _db!.InsertAsync(s);

        var products = new List<Product>
        {
            new() { SKU = "ELEC-001", Name = "Wireless Headphones",     CategoryId = 1, SupplierId = 1, Quantity = 45,  ReorderPoint = 10, ReorderQty = 50,  UnitPrice = 79.99m  },
            new() { SKU = "ELEC-002", Name = "USB-C Hub 7-Port",        CategoryId = 1, SupplierId = 1, Quantity = 8,   ReorderPoint = 15, ReorderQty = 30,  UnitPrice = 34.99m  },
            new() { SKU = "ELEC-003", Name = "Mechanical Keyboard",     CategoryId = 1, SupplierId = 1, Quantity = 0,   ReorderPoint = 5,  ReorderQty = 20,  UnitPrice = 129.99m },
            new() { SKU = "CLTH-001", Name = "Cotton T-Shirt (M)",      CategoryId = 2, SupplierId = 2, Quantity = 120, ReorderPoint = 30, ReorderQty = 100, UnitPrice = 12.99m  },
            new() { SKU = "CLTH-002", Name = "Denim Jeans (32)",        CategoryId = 2, SupplierId = 2, Quantity = 6,   ReorderPoint = 20, ReorderQty = 50,  UnitPrice = 49.99m  },
            new() { SKU = "FOOD-001", Name = "Organic Coffee Beans 1kg",CategoryId = 3, SupplierId = 3, Quantity = 35,  ReorderPoint = 20, ReorderQty = 60,  UnitPrice = 18.99m  },
            new() { SKU = "FOOD-002", Name = "Green Tea Bags x50",      CategoryId = 3, SupplierId = 3, Quantity = 3,   ReorderPoint = 25, ReorderQty = 100, UnitPrice = 8.49m   },
            new() { SKU = "TOOL-001", Name = "Cordless Drill 18V",      CategoryId = 4, SupplierId = 4, Quantity = 22,  ReorderPoint = 5,  ReorderQty = 15,  UnitPrice = 89.99m  },
            new() { SKU = "TOOL-002", Name = "Screwdriver Set 12pc",    CategoryId = 4, SupplierId = 4, Quantity = 58,  ReorderPoint = 10, ReorderQty = 40,  UnitPrice = 24.99m  },
            new() { SKU = "OFFC-001", Name = "A4 Paper Ream 500pg",     CategoryId = 5, SupplierId = 1, Quantity = 14,  ReorderPoint = 20, ReorderQty = 50,  UnitPrice = 6.99m   },
        };
        foreach (var p in products) await _db!.InsertAsync(p);

        var transactions = new List<StockTransaction>
        {
            new() { ProductId = 1, Delta = 50,  Reason = "Purchase Order", Notes = "PO-0001 received", Date = DateTime.Now.AddDays(-10) },
            new() { ProductId = 1, Delta = -5,  Reason = "Sale",           Notes = "Order #1042",      Date = DateTime.Now.AddDays(-7)  },
            new() { ProductId = 2, Delta = 30,  Reason = "Purchase Order", Notes = "PO-0002 received", Date = DateTime.Now.AddDays(-8)  },
            new() { ProductId = 2, Delta = -22, Reason = "Sale",           Notes = "Bulk order #1039", Date = DateTime.Now.AddDays(-3)  },
            new() { ProductId = 4, Delta = 200, Reason = "Purchase Order", Notes = "PO-0003 received", Date = DateTime.Now.AddDays(-5)  },
            new() { ProductId = 4, Delta = -80, Reason = "Sale",           Notes = "Weekly sales",     Date = DateTime.Now.AddDays(-1)  },
            new() { ProductId = 3, Delta = -20, Reason = "Sale",           Notes = "Last batch sold",  Date = DateTime.Now.AddDays(-2)  },
        };
        foreach (var t in transactions) await _db!.InsertAsync(t);
    }
}

// ── Dashboard Stats DTO ───────────────────────────────────────
public class DashboardStats
{
    public int TotalProducts { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int PendingOrders { get; set; }
    public List<StockTransaction> RecentTransactions { get; set; } = new();
}