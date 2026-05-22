using Catalog.Data;
using Catalog.Data.Seed;
using Catalog.Endpoint.Handlers;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Catalog.Endpoint;

// Single registration entry point shared by Catalog's own Program.cs (Compound A,
// each endpoint in its own process) and OmNomNom.AllInOne (Compound B, every
// endpoint in one process). Assembly scanning is disabled here so the AllInOne
// host - which has all six endpoints' [Handler] types in its load context -
// doesn't try to register Finance/Shipping/etc. handlers into the Catalog
// endpoint configuration. Standalone Catalog is unaffected: it only loads its
// own handler types.
public static class CatalogEndpointHost
{
    const string EndpointName = "Catalog";

    public static void Register(IHostApplicationBuilder builder)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("catalog");

        builder.Services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);
        endpointConfiguration.AddHandler<CompleteOrderHandler>();

        builder.Services.AddNServiceBusEndpoint(endpointConfiguration, EndpointName);
    }

    public static async Task InitializeDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext);
    }
}
