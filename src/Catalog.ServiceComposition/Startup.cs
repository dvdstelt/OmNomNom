using Catalog.Data;
using Catalog.ServiceComposition.Helpers;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        // Read-only access from the gateway. Schema creation and seeding
        // are owned by Catalog.Endpoint; this side just expects the
        // database file to already exist when the first request lands.
        options.Services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("catalog")));
        options.Services.AddSingleton<CacheHelper>();
    }
}
