# StockFlow — Inventory Management Web App

A modern inventory management web application built with **.NET MAUI Blazor Hybrid** and **Blazor Server**, featuring a sleek dark theme with red accents.

![StockFlow Dashboard](https://images.unsplash.com/photo-1553413077-190dd305871c?w=1200&q=80)

## 🚀 Features

- **Dashboard** — Real-time KPI cards, low stock alerts, recent activity feed
- **Inventory** — Product management with search, sort, and filter by stock status
- **Purchase Orders** — Create and receive supplier orders
- **Suppliers** — Manage supplier contacts and details
- **Analytics** — Bar charts, donut charts, and top product value rankings
- **Stock Adjustment** — Add or remove stock with reason tracking
- **SQLite Database** — Local persistent storage with seed data
- **Dark Theme** — Sleek black/red UI with gradient cards and glow effects
- ## 🎨 UI Features

- **Croell-inspired theme** — White navbar, red accents, dark content areas
- **Global search** — Real-time product search from any page with dropdown results
- **Interactive charts** — Bar chart with hover effects, donut chart with segment highlighting
- **KPI card navigation** — Click cards to navigate to filtered inventory views
- **Background images** — Contextual images blended into KPI cards
- **Custom SVG logo** — Black/red cube with bar chart icon
- **Responsive layout** — Full-width cards, sticky navbar

## 📅 Changelog

### v1.1 (May 19, 2026)
- Added global search bar with product dropdown
- Redesigned theme to Croell-style white/red/black
- Added hover effects on donut chart segments
- KPI cards now clickable with navigation
- Background images on dashboard cards
- Custom black/red SVG logo
- Improved badge readability in inventory

### v1.0 (May 18, 2026)
- Initial release
- Full inventory management with SQLite
- Dashboard, Inventory, Orders, Suppliers, Analytics pages
- Dark theme with red accents

## 🛠 Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET 10, MAUI Blazor Hybrid |
| UI | Blazor Server, Razor Components |
| Database | SQLite (sqlite-net-pcl) |
| Styling | Custom CSS (no Bootstrap) |
| Charts | SVG-based custom charts |

## 📁 Project Structure

StockFlowWeb/
├── StockFlowWeb/          # MAUI host project
│   ├── MauiProgram.cs
│   ├── MainPage.xaml
│   └── wwwroot/
├── StockFlowWeb.Shared/   # Shared class library
│   ├── Models/            # Product, Category, Supplier, etc.
│   ├── Services/          # DatabaseService, CsvExportService
│   ├── Layout/            # NavMenu, MainLayout
│   └── wwwroot/           # CSS, assets
└── StockFlowWeb.Web/      # ASP.NET Blazor Server
├── Components/
│   └── Pages/         # All Razor pages
├── Services/
└── Program.cs

## 📦 Models

- **Product** — SKU, name, category, supplier, quantity, price, reorder point
- **Category** — Name, color, icon
- **Supplier** — Name, email, phone, address
- **StockTransaction** — Product, delta, reason, date
- **PurchaseOrder** — Supplier, status, items, dates
- **PurchaseOrderItem** — Product, quantity, unit cost

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022 (v17.8+)
- .NET 10 SDK
- MAUI workload installed

### Run the Web App

1. Clone the repo:
```bash
git clone https://github.com/ajayindla/StockFlowWeb.git
```

2. Open `StockFlowWeb.sln` in Visual Studio

3. Set `StockFlowWeb.Web` as startup project

4. Restore NuGet packages:
```bash
dotnet restore
```

5. Run:
```bash
dotnet run --project StockFlowWeb.Web
```

6. Open browser at `http://localhost:5089`

## 📸 Screenshots

### Dashboard
- 4 KPI cards with gradient backgrounds and background images
- Low stock alerts list
- Recent transaction activity feed

### Inventory
- Card-per-product layout
- Search by name or SKU
- Sort by Name, Stock, Value, or Low Stock
- Adjust stock, edit, and delete actions

### Analytics
- Bar chart for stock levels
- Donut chart for value by category with hover effects
- Top 5 products by value with progress bars

## 🗄 Database

SQLite database is auto-created on first run with seed data including:
- 5 categories (Electronics, Clothing, Food & Beverage, Tools, Office Supplies)
- 4 suppliers
- 10 sample products
- 7 stock transactions

## 🔧 Configuration

Database path is configured per host:
- **MAUI**: `FileSystem.AppDataDirectory/stockflow.db3`
- **Web**: `Environment.SpecialFolder.LocalApplicationData/stockflow.db3`

## 📄 License

MIT License — feel free to use and modify.

## 👨‍💻 Author

**Ajay Indla**
- GitHub: [@ajayindla](https://github.com/ajayindla)
- LinkedIn: [Ajay Indla](https://linkedin.com/in/ajayindla)
