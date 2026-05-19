using StockFlowWeb.Shared.Services;

namespace StockFlowWeb.Services;

public class MauiDatabasePath : IDatabasePath
{
    public string GetPath() =>
        Path.Combine(FileSystem.AppDataDirectory, "stockflow.db3");
}