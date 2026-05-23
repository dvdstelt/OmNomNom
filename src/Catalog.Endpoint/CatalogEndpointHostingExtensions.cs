using Catalog.Data;
using Catalog.Data.Seed;
using Catalog.Endpoint.Handlers;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

// Single registration entry point shared by Catalog's own Program.cs (Compound A,
// each endpoint in its own process) and OmNomNom.AllInOne (Compound B, every
// endpoint in one process). Assembly scanning is disabled here so the AllInOne
// host - which has all six endpoints' [Handler] types in its load context -
// doesn't try to register Finance/Shipping/etc. handlers into the Catalog
// endpoint configuration. Standalone Catalog is unaffected: it only loads its
// own handler types.
public static class CatalogEndpointHostingExtensions
{
    public static IServiceCollection AddCatalogEndpoint(this IServiceCollection services)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("catalog");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        services.AddHostedService<CatalogDatabaseSeeder>();

        var endpointConfiguration = new NServiceBus.EndpointConfiguration("Catalog");
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);
        endpointConfiguration.AddHandler<CompleteOrderHandler>();

        services.AddNServiceBusEndpoint(endpointConfiguration, "Catalog");
        return services;
    }
}

sealed class CatalogDatabaseSeeder(IServiceProvider services) : DatabaseSeederHostedService(services)
{
    protected override async Task SeedAsync(IServiceProvider scopedServices, CancellationToken cancellationToken)
    {
        var dbContext = scopedServices.GetRequiredService<CatalogDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext, cancellationToken);
    }
}
