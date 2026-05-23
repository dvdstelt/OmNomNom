using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Marketing.Data;
using Marketing.Data.Seed;
using Marketing.Endpoint;
using Marketing.Endpoint.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

// Shared registration entry point - see CatalogEndpointHostingExtensions
// for the rationale behind the disabled assembly scanner and the
// hosted database seeder.
public static class MarketingEndpointHostingExtensions
{
    public static IServiceCollection AddMarketingEndpoint(this IServiceCollection services)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("marketing");

        services.AddDbContext<MarketingDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        services.AddHostedService<MarketingDatabaseSeeder>();

        // Marketing only subscribes to events, so no command routing is needed -
        // LearningTransport's pub/sub handles the rest via the shared
        // .learningtransport folder.
        var endpointConfiguration = new NServiceBus.EndpointConfiguration("Marketing");
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);
        endpointConfiguration.AddHandler<OrderPlacedHandler>();

        services.AddNServiceBusEndpoint(endpointConfiguration, "Marketing");

        // Background recompute that keeps Marketing.Product.Trending honest as
        // seeded events age out of the 30-day window.
        services.AddHostedService<TrendingRecomputeService>();

        return services;
    }
}

sealed class MarketingDatabaseSeeder(IServiceProvider services) : DatabaseSeederHostedService(services)
{
    protected override async Task SeedAsync(IServiceProvider scopedServices, CancellationToken cancellationToken)
    {
        var dbContext = scopedServices.GetRequiredService<MarketingDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext, cancellationToken);
    }
}
