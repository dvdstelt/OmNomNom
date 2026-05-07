using ITOps.Shared.Sqlite;
using Marketing.Contracts;
using Marketing.Data;
using Marketing.ServiceComposition.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        // Marketing's database name is "marketing", not "shipping" — the
        // pre-SQLite Startup pointed MarketingDbContext at "shipping",
        // which made Marketing share the Shipping LiteDB file. Fixed
        // here as part of the migration so each service genuinely owns
        // its own SQLite file under src/.db/.
        options.Services.AddDbContext<MarketingDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("marketing")));

        // Cross-service contract: Catalog asks Marketing for sorted
        // ProductId lists when sorting on Marketing-owned signals
        // (rating, orderCount, trending). The implementation lives
        // in Marketing.ServiceComposition; the interface in the
        // contracts library so Catalog never touches Marketing.Data.
        options.Services.AddScoped<IProductRanker, ProductRanker>();

        // No separate Marketing.Endpoint, so the gateway has to run
        // migrate + seed itself.
        options.Services.AddHostedService<MarketingDatabaseInitializer>();
    }
}
