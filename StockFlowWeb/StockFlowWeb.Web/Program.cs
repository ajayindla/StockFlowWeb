using StockFlowWeb.Web.Services;
using StockFlowWeb.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IDatabasePath, WebDatabasePath>();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<CsvExportService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<StockFlowWeb.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();