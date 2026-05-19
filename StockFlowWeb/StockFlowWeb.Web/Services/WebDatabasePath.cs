using StockFlowWeb.Shared.Services;

namespace StockFlowWeb.Web.Services;

public class WebDatabasePath : IDatabasePath
{
    public string GetPath() =>
        Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "stockflow.db3");
}