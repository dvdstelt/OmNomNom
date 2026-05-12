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
        // Read-only access to the marketing domain DB from the gateway.
        // Schema creation and seeding are owned by Marketing.Endpoint.
        options.Services.AddDbContext<MarketingDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("marketing")));
    }
}
