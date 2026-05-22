using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Marketing.Data;
using Marketing.Data.Seed;
using Marketing.Endpoint.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Marketing.Endpoint;

// Shared registration entry point - see CatalogEndpointHost for the
// rationale behind the disabled assembly scanner.
public static class MarketingEndpointHost
{
    const string EndpointName = "Marketing";

    public static void Register(IHostApplicationBuilder builder)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("marketing");

        builder.Services.AddDbContext<MarketingDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        // Marketing only subscribes to events, so no command routing is needed -
        // LearningTransport's pub/sub handles the rest via the shared
        // .learningtransport folder.
        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);
        endpointConfiguration.AddHandler<OrderPlacedHandler>();

        builder.Services.AddNServiceBusEndpoint(endpointConfiguration);

        // Background recompute that keeps Marketing.Product.Trending honest as
        // seeded events age out of the 30-day window.
        builder.Services.AddHostedService<TrendingRecomputeService>();
    }

    public static async Task InitializeDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MarketingDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext);
    }
}
