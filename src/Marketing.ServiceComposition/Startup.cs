using ITOps.Shared.Sqlite;
using Marketing.Data;
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

        // Run EnsureCreated + seed in the gateway process too, alongside
        // the standalone Marketing.Endpoint. EnsureCreated is idempotent
        // and the seed only runs when Products is empty, so two writers
        // racing against the same SQLite file is harmless and keeps the
        // gateway usable when Marketing.Endpoint isn't started yet.
        options.Services.AddHostedService<MarketingDatabaseInitializer>();
    }
}
