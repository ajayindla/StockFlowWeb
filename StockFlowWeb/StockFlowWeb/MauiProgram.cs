using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using StockFlowWeb.Services;
using StockFlowWeb.Shared.Services;

namespace StockFlowWeb;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IDatabasePath, MauiDatabasePath>();
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<CsvExportService>();

        return builder.Build();
    }
}